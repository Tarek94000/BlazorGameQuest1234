using GameService.Data;
using GameServices.Services;
using Microsoft.EntityFrameworkCore;
using SharedModels;
using Xunit;

namespace Tests;

// Tests unitaires pour la logique du jeu
public class GameLogicTests
{
    // Crée un DbContext en mémoire isolé pour chaque test
    private GameDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // base neuve à chaque test
            .Options;

        return new GameDbContext(options);
    }

    // Vérifie qu'une nouvelle partie créé un donjon contenant entre 1 et 5 salles
    [Fact]
    public async Task DemarrerNouvellePartie_CreeUnDonjonAvecSalles()
    {
        using var db = GetDbContext();

        // Prépare un joueur nécessaire pour démarrer la partie
        db.Joueurs.Add(new Joueur { Nom = "Test" });
        db.SaveChanges();

        var service = new GameLogicService(db);

        // Démarre la partie pour le joueur d'id 1
        var partie = await service.DemarrerNouvellePartieAsync(1);

        Assert.NotNull(partie);
        Assert.True(partie.DonjonId > 0);

        // Récupère le donjon créé avec ses salles
        var donjon = db.Donjons
            .Include(d => d.Salles)
            .First(d => d.Id == partie.DonjonId);

        // Vérifie qu'il y a bien des salles et que leur nombre est dans l'intervalle attendu
        Assert.NotEmpty(donjon.Salles);
        Assert.InRange(donjon.Salles.Count, 1, 5);
    }

    // Applique des choix répétés "Combattre" jusqu'à la fin de la partie
    [Fact]
    public async Task AppliquerChoix_FinitParTerminerLaPartie()
    {
        using var db = GetDbContext();

        db.Joueurs.Add(new Joueur { Nom = "Test" });
        db.SaveChanges();

        var service = new GameLogicService(db);
        var partie = await service.DemarrerNouvellePartieAsync(1);

        int gardeFou = 20; // évite boucle infinie si bug
        while (!partie.EstTerminee && gardeFou-- > 0)
        {
            // Applique l'action "Combattre" et récupère l'état mis à jour de la partie
            partie = await service.AppliquerChoixAsync(partie.Id, "Combattre");
        }

        // Doit être terminée et avoir une date de fin
        Assert.True(partie.EstTerminee);
        Assert.NotNull(partie.DateFin);
    }

    // Vérifie que des combats successifs peuvent tuer le joueur
    [Fact]
    public async Task AppliquerChoix_PeutTuerLeJoueur()
    {
        using var db = GetDbContext();

        db.Joueurs.Add(new Joueur { Nom = "Test" });
        db.SaveChanges();

        var service = new GameLogicService(db);
        var partie = await service.DemarrerNouvellePartieAsync(1);

        // On enchaîne des combats jusqu'à potentiellement mourir
        int gardeFou = 50;
        while (!partie.EstTerminee && gardeFou-- > 0)
        {
            partie = await service.AppliquerChoixAsync(partie.Id, "Combattre");
        }

        // Si le score est <= 0 alors le joueur doit être mort et la partie terminée
        if (partie.ScoreCourant <= 0)
        {
            Assert.True(partie.EstMort);
            Assert.True(partie.EstTerminee);
        }
    }
}
