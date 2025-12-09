using System;
using System.Net.Http;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using BlazorGame.Client;
using BlazorGame.Client.Security;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// 🔹 HttpClient qui parle à GameServices et ajoute le token automatiquement
builder.Services.AddHttpClient("GameApi", client =>
    client.BaseAddress = new Uri("http://localhost:5297"))   // ⚠️ même URL que GameServices
    .AddHttpMessageHandler(sp =>
    {
        var handler = sp.GetRequiredService<AuthorizationMessageHandler>()
            .ConfigureHandler(
                authorizedUrls: new[] { "http://localhost:5297" },
                scopes: new[] { "openid", "profile", "email" }
            );

        return handler;
    });

// 🔹 HttpClient PUBLIC pour les invités (sans token)
builder.Services.AddHttpClient("PublicGameApi", client =>
    client.BaseAddress = new Uri("http://localhost:5297"));

// HttpClient injecté par défaut = client "GameApi"
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("GameApi"));


// 🔹 Auth OIDC (Keycloak) — AVEC MAPPING DES RÔLES
builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Oidc", options.ProviderOptions);
    options.UserOptions.RoleClaim = "role";
})
.AddAccountClaimsPrincipalFactory<CustomUserFactory>();

await builder.Build().RunAsync();
