using GameService.Data;
using GameServices.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedModels;
using Xunit;

namespace Tests;

public class AdminControllerTests
{
    private GameDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new GameDbContext(options);
    }

    [Fact]
    public async Task GetJoueurs_RetourneLesInfosAdmin()
    {
        using var db = GetDbContext();

        var j1 = new Joueur { Nom = "Alice", EstActif = true };
        var j2 = new Joueur { Nom = "Bob", EstActif = false };
        db.Joueurs.AddRange(j1, j2);
        db.SaveChanges();

        db.Parties.AddRange(
            new Partie { JoueurId = j1.Id, ScoreFinal = 10, DateDebut = DateTime.Now },
            new Partie { JoueurId = j1.Id, ScoreFinal = 5, DateDebut = DateTime.Now },
            new Partie { JoueurId = j2.Id, ScoreFinal = 7, DateDebut = DateTime.Now }
        );
        db.SaveChanges();

        var controller = new AdminController(db);

        var result = await controller.GetJoueurs();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var joueurs = Assert.IsAssignableFrom<IEnumerable<AdminJoueurDto>>(ok.Value).ToList();

        Assert.Equal(2, joueurs.Count);
        var alice = joueurs.Single(j => j.Nom == "Alice");
        var bob = joueurs.Single(j => j.Nom == "Bob");

        Assert.Equal(2, alice.NombreParties);
        Assert.Equal(1, bob.NombreParties);
    }

    [Fact]
    public async Task DesactiverJoueur_MetEstActifAFaux()
    {
        using var db = GetDbContext();

        var joueur = new Joueur { Nom = "Test", EstActif = true };
        db.Joueurs.Add(joueur);
        db.SaveChanges();

        var controller = new AdminController(db);

        var result = await controller.DesactiverJoueur(joueur.Id);

        Assert.IsType<NoContentResult>(result);

        var updated = db.Joueurs.Find(joueur.Id);
        Assert.False(updated!.EstActif);
    }

    [Fact]
    public async Task ActiverJoueur_MetEstActifAVrai()
    {
        using var db = GetDbContext();

        var joueur = new Joueur { Nom = "Test", EstActif = false };
        db.Joueurs.Add(joueur);
        db.SaveChanges();

        var controller = new AdminController(db);

        var result = await controller.ActiverJoueur(joueur.Id);

        Assert.IsType<NoContentResult>(result);

        var updated = db.Joueurs.Find(joueur.Id);
        Assert.True(updated!.EstActif);
    }
    [Fact]
    public async Task GetParties_ReturnsAllParties()
    {
        using var db = GetDbContext();
        var j = new Joueur { Nom = "J1" };
        var d = new Donjon { Nom = "D1" };
        db.Joueurs.Add(j);
        db.Donjons.Add(d);
        db.SaveChanges();

        db.Parties.Add(new Partie { JoueurId = j.Id, DonjonId = d.Id, ScoreFinal = 100 });
        db.SaveChanges();

        var controller = new AdminController(db);
        var result = await controller.GetParties();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var dtos = Assert.IsAssignableFrom<IEnumerable<AdminPartieDto>>(ok.Value);
        Assert.Single(dtos);
        Assert.Equal("J1", dtos.First().JoueurNom);
    }

    [Fact]
    public async Task ExportJoueursCsv_ReturnsFile()
    {
        using var db = GetDbContext();
        db.Joueurs.Add(new Joueur { Nom = "JExport", Score = 50 });
        db.SaveChanges();

        var controller = new AdminController(db);
        var result = await controller.ExportJoueursCsv();

        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("text/csv", fileResult.ContentType);
        
        var content = System.Text.Encoding.UTF8.GetString(fileResult.FileContents);
        Assert.Contains("Id;Nom;Score;EstActif;NombreParties", content);
        Assert.Contains("JExport;50", content);
    }
}
