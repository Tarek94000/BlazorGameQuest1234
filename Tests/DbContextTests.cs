using Microsoft.EntityFrameworkCore;
using GameService.Data;
using SharedModels;
using Xunit;

namespace Tests;

public class DbContextTests
{
    [Fact]
    public void CanAddJoueurToDatabase()
    {
        // Crée un DbContext InMemory pour le test
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        using var context = new GameDbContext(options);
        context.Joueurs.Add(new Joueur { Nom = "Tarek" });
        context.SaveChanges();

        var joueur = context.Joueurs.FirstOrDefault(j => j.Nom == "Tarek");

        Assert.NotNull(joueur);
        Assert.Equal("Tarek", joueur!.Nom);
    }
}
