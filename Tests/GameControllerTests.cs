using GameService.Data;
using GameServices.Controllers;
using GameServices.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedModels;
using System.Security.Claims;
using Xunit;

namespace Tests;

public class GameControllerTests
{
    private GameDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new GameDbContext(options);
    }

    private GameLogicService GetGameLogicService(GameDbContext db)
    {
        return new GameLogicService(db);
    }

    private GameController GetController(GameDbContext db, ClaimsPrincipal? user = null)
    {
        var logic = GetGameLogicService(db);
        var controller = new GameController(logic, db);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user ?? new ClaimsPrincipal() }
        };

        return controller;
    }

    [Fact]
    public async Task Start_WithJoueurId_ReturnsGameState()
    {
        using var db = GetDbContext();
        var joueur = new Joueur { Nom = "TestJoueur", Score = 0, EstActif = true };
        db.Joueurs.Add(joueur);
        // Add a dungeon for the game logic to work
        db.Donjons.Add(new Donjon { Nom = "TestDonjon", Difficulte = 1 });
        db.SaveChanges();

        var controller = GetController(db);

        var result = await controller.Start(joueur.Id);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var state = Assert.IsType<GameStateDto>(okResult.Value);
        
        Assert.Equal(joueur.Id, state.JoueurId);
        Assert.False(state.EstTerminee);
    }

    [Fact]
    public async Task Start_WithAuthenticatedUser_CreatesAndReturnsGameState()
    {
        using var db = GetDbContext();
        db.Donjons.Add(new Donjon { Nom = "TestDonjon", Difficulte = 1 });
        db.SaveChanges();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "keycloak-123"),
            new Claim(ClaimTypes.Name, "KeycloakUser")
        }, "TestAuth"));

        var controller = GetController(db, user);

        var result = await controller.Start(null);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var state = Assert.IsType<GameStateDto>(okResult.Value);

        var dbJoueur = await db.Joueurs.FirstOrDefaultAsync(j => j.KeycloakId == "keycloak-123");
        Assert.NotNull(dbJoueur);
        Assert.Equal("KeycloakUser", dbJoueur.Nom);
        Assert.Equal(dbJoueur.Id, state.JoueurId);
    }

    [Fact]
    public async Task GetState_ReturnsGameState()
    {
        using var db = GetDbContext();
        var joueur = new Joueur { Nom = "TestJoueur", Score = 0, EstActif = true };
        db.Joueurs.Add(joueur);
        var donjon = new Donjon { Nom = "TestDonjon", Difficulte = 1 };
        db.Donjons.Add(donjon);
        db.SaveChanges();

        var logic = GetGameLogicService(db);
        var partie = await logic.DemarrerNouvellePartieAsync(joueur.Id);

        var controller = GetController(db);
        var result = await controller.GetState(partie.Id);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var state = Assert.IsType<GameStateDto>(okResult.Value);
        Assert.Equal(partie.Id, state.PartieId);
    }

    [Fact]
    public async Task ApplyChoice_ReturnsUpdatedState()
    {
        using var db = GetDbContext();
        var joueur = new Joueur { Nom = "TestJoueur", Score = 0, EstActif = true };
        db.Joueurs.Add(joueur);
        var donjon = new Donjon { Nom = "TestDonjon", Difficulte = 1 };
        db.Donjons.Add(donjon);
        db.SaveChanges();

        var logic = GetGameLogicService(db);
        var partie = await logic.DemarrerNouvellePartieAsync(joueur.Id);
        
        // Ensure we are in a room or start state
        // Usually game starts at index 0. We need to check what choices are valid.
        // Assuming "Attaquer" or something is valid if logic allows, but here we just test the flow.
        // We might get "Partie terminée" or similar depending on logic, but we just check we get a result.
        
        var controller = GetController(db);
        var request = new ChoiceRequest { Choix = "Ouvrir" }; // Valid choice depends on room
        
        var result = await controller.ApplyChoice(partie.Id, request);

        // Logic service might return null if choice invalid or game over, 
        // but checking GameController.ApplyChoice implementation:
        // var partie = await _gameLogicService.AppliquerChoixAsync(partieId, request.Choix);
        // if (partie == null) return NotFound...
        
        // If choice is invalid, logic usually returns the partie with updated message or same state.
        
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var state = Assert.IsType<GameStateDto>(okResult.Value);
        Assert.Equal(partie.Id, state.PartieId);
    }

    [Fact]
    public async Task Start_NoUserNoId_ReturnsBadRequest()
    {
        using var db = GetDbContext();
        var controller = GetController(db); // No user

        var result = await controller.Start(null);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
}
