using GameService.Controllers;
using GameService.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedModels;
using Xunit;

namespace Tests;

public class DonjonsControllerTests
{
    private GameDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new GameDbContext(options);
    }

    [Fact]
    public async Task GetAll_ReturnsDonjons()
    {
        using var db = GetDbContext();
        db.Donjons.Add(new Donjon { Nom = "D1" });
        db.Donjons.Add(new Donjon { Nom = "D2" });
        db.SaveChanges();

        var controller = new DonjonsController(db);
        var result = await controller.GetAll();

        var donjons = Assert.IsAssignableFrom<IEnumerable<Donjon>>(result.Value);
        Assert.Equal(2, donjons.Count());
    }

    [Fact]
    public async Task Create_AddsDonjon()
    {
        using var db = GetDbContext();
        var controller = new DonjonsController(db);
        var donjon = new Donjon { Nom = "NewD", Difficulte = 2 };

        var result = await controller.Create(donjon);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdDonjon = Assert.IsType<Donjon>(createdResult.Value);
        Assert.Equal("NewD", createdDonjon.Nom);

        Assert.Equal(1, db.Donjons.Count());
    }
}
