using GameService.Data;
using GameServices.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using SharedModels;

var builder = WebApplication.CreateBuilder(args);

// =========================
// 1. EF Core InMemory
// =========================
builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseInMemoryDatabase("GameQuestDb"));

// =========================
// 2. Logique de jeu
// =========================
builder.Services.AddScoped<GameLogicService>();

// =========================
// 3. Authentification & Autorisation (Keycloak)
// =========================
//
// Hypothèses :
// - Realm Keycloak : blazorgame
// - URL Keycloak : http://localhost:8080
// - Client API : game-api
//
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "http://localhost:8080/realms/blazorgame";
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateAudience = false,
            // ValidAudiences = new[] { "blazor-client", "account" },
            ValidateIssuer = false, 
            // ValidIssuer = "http://localhost:8080/realms/blazorgame",
            NameClaimType = "preferred_username"
        };
        
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"[GameServices] Authentication Failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                // DEBUG: Inspecter l'identité reçue
                var principal = context.Principal;
                if (principal is not null)
                {
                    Console.WriteLine($"[GameServices] User Authenticated: {principal.Identity?.Name}");
                    foreach (var claim in principal.Claims)
                    {
                        Console.WriteLine($"[GameServices] Claim: {claim.Type} = {claim.Value}");
                    }
                }

                if (context.SecurityToken is System.IdentityModel.Tokens.Jwt.JwtSecurityToken jwt)
                {
                    if (principal?.Identity is System.Security.Claims.ClaimsIdentity identity)
                    {
                        // 1. realm_access
                        // Le claim peut être un objet JSON complexe, donc on le cherche dans les claims bruts du JWT
                        var realmAccess = jwt.Claims.FirstOrDefault(c => c.Type == "realm_access")?.Value;
                        if (!string.IsNullOrEmpty(realmAccess))
                        {
                            Console.WriteLine($"[GameServices] Found realm_access: {realmAccess}");
                            try 
                            {
                                using var doc = System.Text.Json.JsonDocument.Parse(realmAccess);
                                if (doc.RootElement.TryGetProperty("roles", out var roles))
                                {
                                    foreach (var role in roles.EnumerateArray())
                                    {
                                        var r = role.GetString();
                                        if (!string.IsNullOrEmpty(r))
                                        {
                                            Console.WriteLine($"[GameServices] Adding Role: {r}");
                                            identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, r));
                                        }
                                    }
                                }
                            }
                            catch (Exception ex) 
                            { 
                                Console.WriteLine($"[GameServices] Error parsing realm_access: {ex.Message}");
                            }
                        }
                        
                        // 2. resource_access
                        var resourceAccess = jwt.Claims.FirstOrDefault(c => c.Type == "resource_access")?.Value;
                        if (!string.IsNullOrEmpty(resourceAccess))
                        {
                            Console.WriteLine($"[GameServices] Found resource_access: {resourceAccess}");
                             try
                             {
                                 using var doc = System.Text.Json.JsonDocument.Parse(resourceAccess);
                                 foreach (var client in doc.RootElement.EnumerateObject())
                                 {
                                     if (client.Value.TryGetProperty("roles", out var roles))
                                     {
                                         foreach (var role in roles.EnumerateArray())
                                         {
                                            var r = role.GetString();
                                            if (!string.IsNullOrEmpty(r))
                                            {
                                                Console.WriteLine($"[GameServices] Adding Client Role: {r}");
                                                identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, r));
                                            }
                                         }
                                     }
                                 }
                             }
                             catch (Exception ex)
                             {
                                 Console.WriteLine($"[GameServices] Error parsing resource_access: {ex.Message}");
                             }
                        }
                    }
                }
                
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    // Politique pour les joueurs : simple authentification suffit pour commencer
    options.AddPolicy("Joueur", policy =>
    {
        policy.RequireAuthenticatedUser();
    });

    // Politique pour les admins
    options.AddPolicy("Admin", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("admin");
    });
});

// =========================
// 4. CORS (Blazor client)
// =========================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("http://localhost:5000", "https://localhost:5000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// =========================
// 5. MVC + Swagger
// =========================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// =========================
// 6. Seed : créer un joueur "Invité" si la table est vide
// =========================
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<GameDbContext>();

    if (!context.Joueurs.Any())
    {
        var guest = new Joueur
        {
            Nom = "Invité",
            EstActif = true,
            Score = 0,
            KeycloakId = "guest"
        };

        context.Joueurs.Add(guest);
        context.SaveChanges();
    }
}

// =========================
// 7. Pipeline HTTP
// =========================
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowBlazorClient");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
