using GameService.Controllers;
using GameService.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedModels;
using Xunit;

namespace Tests;

public class PartiesControllerTests
{
    private GameDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new GameDbContext(options);
    }

    [Fact]
    public async Task GetAll_ReturnsParties()
    {
        using var db = GetDbContext();
        var j = new Joueur { Nom = "J1" };
        var d = new Donjon { Nom = "D1" };
        db.Joueurs.Add(j);
        db.Donjons.Add(d);
        db.SaveChanges(); // Persist parents first

        db.Parties.Add(new Partie { ScoreCourant = 10, JoueurId = j.Id, DonjonId = d.Id });
        db.Parties.Add(new Partie { ScoreCourant = 20, JoueurId = j.Id, DonjonId = d.Id });
        db.SaveChanges();

        var controller = new PartiesController(db);
        var result = await controller.GetAll();

        var parties = Assert.IsAssignableFrom<IEnumerable<Partie>>(result.Value);
        Assert.Equal(2, parties.Count());
    }

    [Fact]
    public async Task Create_AddsPartie()
    {
        using var db = GetDbContext();
        var controller = new PartiesController(db);
        var partie = new Partie { ScoreCourant = 100 };

        var result = await controller.Create(partie);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdPartie = Assert.IsType<Partie>(createdResult.Value);
        Assert.Equal(100, createdPartie.ScoreCourant);

        Assert.Equal(1, db.Parties.Count());
    }
}
