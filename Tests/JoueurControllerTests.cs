using GameService.Data;
using GameServices.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedModels;
using Xunit;

namespace Tests;

public class JoueursControllerTests
{
    private GameDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new GameDbContext(options);
    }

    [Fact]
    public async Task GetHistorique_RetourneLesPartiesDuJoueur()
    {
        // Arrange
        using var db = GetDbContext();

        var joueur = new Joueur { Nom = "Test" };
        db.Joueurs.Add(joueur);

        var donjon = new Donjon { Nom = "Donjon Test", Difficulte = 2 };
        db.Donjons.Add(donjon);
        db.SaveChanges();

        db.Parties.AddRange(
            new Partie
            {
                JoueurId = joueur.Id,
                DonjonId = donjon.Id,
                ScoreFinal = 10,
                DateDebut = DateTime.Now.AddMinutes(-10),
                DateFin = DateTime.Now.AddMinutes(-5),
                EtatPartie = "Terminee"
            },
            new Partie
            {
                JoueurId = joueur.Id,
                DonjonId = donjon.Id,
                ScoreFinal = 5,
                DateDebut = DateTime.Now.AddMinutes(-4),
                DateFin = null,
                EtatPartie = "EnCours"
            }
        );
        db.SaveChanges();

        var controller = new JoueursController(db);

        // Act
        var result = await controller.GetHistorique(joueur.Id);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var parties = Assert.IsAssignableFrom<IEnumerable<PartieHistoriqueDto>>(ok.Value);

        Assert.Equal(2, parties.Count());
        Assert.Contains(parties, p => p.ScoreFinal == 10);
        Assert.Contains(parties, p => p.ScoreFinal == 5);
    }

    [Fact]
    public async Task GetHistorique_RetourneNotFoundSiJoueurInexistant()
    {
        using var db = GetDbContext();
        var controller = new JoueursController(db);

        var result = await controller.GetHistorique(999);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }
    [Fact]
    public async Task GetAll_ReturnsJoueurs()
    {
        using var db = GetDbContext();
        db.Joueurs.Add(new Joueur { Nom = "J1" });
        db.Joueurs.Add(new Joueur { Nom = "J2" });
        db.SaveChanges();

        var controller = new JoueursController(db);
        var result = await controller.GetAll();

        var joueurs = Assert.IsAssignableFrom<IEnumerable<Joueur>>(result.Value);
        Assert.Equal(2, joueurs.Count());
    }

    [Fact]
    public async Task GetById_ReturnsJoueur()
    {
        using var db = GetDbContext();
        var j = new Joueur { Nom = "J1" };
        db.Joueurs.Add(j);
        db.SaveChanges();

        var controller = new JoueursController(db);
        var result = await controller.GetById(j.Id);

        Assert.Equal("J1", result.Value!.Nom);
    }

    [Fact]
    public async Task Create_AddsJoueur()
    {
        using var db = GetDbContext();
        var controller = new JoueursController(db);
        var j = new Joueur { Nom = "JNew" };

        var result = await controller.Create(j);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdJoueur = Assert.IsType<Joueur>(createdResult.Value);
        Assert.Equal("JNew", createdJoueur.Nom);
    }

    [Fact]
    public async Task Update_ModifiesJoueur()
    {
        using var db = GetDbContext();
        var j = new Joueur { Nom = "JOld" };
        db.Joueurs.Add(j);
        db.SaveChanges();

        var controller = new JoueursController(db);
        j.Nom = "JUpdated";

        var result = await controller.Update(j.Id, j);

        Assert.IsType<NoContentResult>(result);
        var updated = db.Joueurs.Find(j.Id);
        Assert.Equal("JUpdated", updated!.Nom);
    }

    [Fact]
    public async Task Delete_RemovesJoueur()
    {
        using var db = GetDbContext();
        var j = new Joueur { Nom = "JRem" };
        db.Joueurs.Add(j);
        db.SaveChanges();

        var controller = new JoueursController(db);
        var result = await controller.Delete(j.Id);

        Assert.IsType<NoContentResult>(result);
        Assert.Equal(0, db.Joueurs.Count());
    }
}
