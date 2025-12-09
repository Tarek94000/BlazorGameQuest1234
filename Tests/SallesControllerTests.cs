using GameService.Controllers;
using GameService.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedModels;
using Xunit;

namespace Tests;

public class SallesControllerTests
{
    private GameDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new GameDbContext(options);
    }

    [Fact]
    public async Task GetAll_ReturnsSalles()
    {
        using var db = GetDbContext();
        db.Salles.Add(new Salle { Description = "Salle 1" });
        db.Salles.Add(new Salle { Description = "Salle 2" });
        db.SaveChanges();

        var controller = new SallesController(db);
        var result = await controller.GetAll();

        var salles = Assert.IsAssignableFrom<IEnumerable<Salle>>(result.Value);
        Assert.Equal(2, salles.Count());
    }

    [Fact]
    public async Task Create_AddsSalle()
    {
        using var db = GetDbContext();
        var controller = new SallesController(db);
        var salle = new Salle { Description = "New Salle" };

        var result = await controller.Create(salle);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdSalle = Assert.IsType<Salle>(createdResult.Value);
        Assert.Equal("New Salle", createdSalle.Description);

        Assert.Equal(1, db.Salles.Count());
    }
}
