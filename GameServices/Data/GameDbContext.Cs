using Microsoft.EntityFrameworkCore;
using SharedModels;

namespace GameService.Data;

public class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

    public DbSet<Joueur> Joueurs { get; set; }
    public DbSet<Administrateur> Administrateurs { get; set; }
    public DbSet<Donjon> Donjons { get; set; }
    public DbSet<Salle> Salles { get; set; }
    public DbSet<Partie> Parties { get; set; }
}
