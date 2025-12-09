using GameService.Data;
using GameServices.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedModels;
using Xunit;

namespace Tests;

public class ClassementControllerTests
{
    private GameDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new GameDbContext(options);
    }

    [Fact]
    public async Task GetClassement_TrieParMeilleurScore()
    {
        using var db = GetDbContext();

        var joueur1 = new Joueur { Nom = "Alice" };
        var joueur2 = new Joueur { Nom = "Bob" };
        db.Joueurs.AddRange(joueur1, joueur2);
        db.SaveChanges();

        db.Parties.AddRange(
            new Partie { JoueurId = joueur1.Id, ScoreFinal = 10, DateDebut = DateTime.Now },
            new Partie { JoueurId = joueur1.Id, ScoreFinal = 20, DateDebut = DateTime.Now },
            new Partie { JoueurId = joueur2.Id, ScoreFinal = 15, DateDebut = DateTime.Now }
        );
        db.SaveChanges();

        var controller = new ClassementController(db);

        var result = await controller.GetClassement();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var entries = Assert.IsAssignableFrom<IEnumerable<ClassementEntryDto>>(ok.Value).ToList();

        Assert.Equal(2, entries.Count);
        Assert.Equal("Alice", entries[0].Nom); // 20
        Assert.Equal(20, entries[0].MeilleurScore);
        Assert.Equal("Bob", entries[1].Nom);   // 15
    }

    [Fact]
    public async Task GetClassement_RetourneZeroPourJoueurSansParties()
    {
        using var db = GetDbContext();

        var joueur = new Joueur { Nom = "Solo" };
        db.Joueurs.Add(joueur);
        db.SaveChanges();

        var controller = new ClassementController(db);

        var result = await controller.GetClassement();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var entries = Assert.IsAssignableFrom<IEnumerable<ClassementEntryDto>>(ok.Value).ToList();

        Assert.Single(entries);
        Assert.Equal("Solo", entries[0].Nom);
        Assert.Equal(0, entries[0].MeilleurScore);
        Assert.Equal(0, entries[0].NombreParties);
    }
}
