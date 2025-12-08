<<<<<<< HEAD
using GameService.Data;
using GameServices.Services;
using Microsoft.EntityFrameworkCore;
using SharedModels;

var builder = WebApplication.CreateBuilder(args);

// EF Core InMemory
builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseInMemoryDatabase("GameQuestDb"));

// Logique de jeu
builder.Services.AddScoped<GameLogicService>();

// CORS (si tu en as besoin)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("http://localhost:5000", "https://localhost:5000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 🔹 Seed : créer un joueur "Invité" si la table est vide
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

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowBlazorClient");

app.MapControllers();
=======
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/health", () => "ok"); //verification que le service tourne
>>>>>>> origin/prod

app.Run();
