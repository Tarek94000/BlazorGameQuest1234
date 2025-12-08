using GameService.Data;
using Microsoft.EntityFrameworkCore;
using SharedModels;

namespace GameServices.Services;

public class GameLogicService
{
    private readonly GameDbContext _context;
    private readonly Random _random = new();

    public GameLogicService(GameDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Démarre une nouvelle partie pour un joueur donné.
    /// Génère un donjon avec entre 1 et 5 salles.
    /// </summary>
    public async Task<Partie> DemarrerNouvellePartieAsync(int joueurId)
    {
        var joueur = await _context.Joueurs.FindAsync(joueurId);
        if (joueur == null)
            throw new InvalidOperationException($"Joueur {joueurId} introuvable.");

        // Générer un donjon aléatoire
        var donjon = GenererDonjonAleatoire();
        _context.Donjons.Add(donjon);
        await _context.SaveChangesAsync();

        var partie = new Partie
        {
            JoueurId = joueurId,
            DonjonId = donjon.Id,
            ScoreCourant = 0,
            ScoreFinal = 0,
            IndexSalleCourante = 0,
            EstTerminee = false,
            EstMort = false,
            DateDebut = DateTime.Now,
            EtatPartie = "EnCours"
        };

        _context.Parties.Add(partie);
        await _context.SaveChangesAsync();

        return partie;
    }

    /// <summary>
    /// Récupère la salle courante d'une partie.
    /// </summary>
    public async Task<Salle?> ObtenirSalleCouranteAsync(int partieId)
    {
        var partie = await _context.Parties
            .Include(p => p.Donjon)
            .ThenInclude(d => d.Salles)
            .FirstOrDefaultAsync(p => p.Id == partieId);

        if (partie == null || partie.Donjon == null)
            return null;

        if (partie.IndexSalleCourante < 0 || partie.IndexSalleCourante >= partie.Donjon.Salles.Count)
            return null;

        return partie.Donjon.Salles
            .OrderBy(s => s.Id)
            .Skip(partie.IndexSalleCourante)
            .FirstOrDefault();
    }

    /// <summary>
    /// Applique le choix du joueur dans la salle courante.
    /// Met à jour le score, l'état de la partie et avance à la salle suivante si possible.
    /// </summary>
    public async Task<Partie?> AppliquerChoixAsync(int partieId, string choix)
    {
        var partie = await _context.Parties
            .Include(p => p.Donjon)
            .ThenInclude(d => d.Salles)
            .FirstOrDefaultAsync(p => p.Id == partieId);

        if (partie == null || partie.Donjon == null)
            return null;

        if (partie.EstTerminee)
            return partie;

        var salle = await ObtenirSalleCouranteAsync(partieId);
        if (salle == null)
        {
            // Plus de salles → fin de partie
            TerminerPartie(partie);
            await _context.SaveChangesAsync();
            return partie;
        }

        // Normalise le choix
        choix = choix.Trim().ToLowerInvariant();

        // Logique simple : tu pourras l’enrichir
        switch (choix)
        {
            case "combattre":
                // Combat : gros risque, gros gain/perte
                if (Reussite(0.6)) // 60% de chance de succès
                {
                    partie.ScoreCourant += salle.PointsGagnes;
                }
                else
                {
                    partie.ScoreCourant -= salle.PointsPerdus;
                }
                break;

            case "fuir":
                // Fuir : sécurité, mais peu de points
                partie.ScoreCourant += salle.PointsGagnes / 4;
                break;

            case "fouiller":
                // Fouiller : chance de trésor ou piège
                if (Reussite(0.5))
                    partie.ScoreCourant += salle.PointsGagnes;
                else
                    partie.ScoreCourant -= salle.PointsPerdus;
                break;

            default:
                // Choix inconnu : rien ne se passe
                break;
        }

        // Vérifier mort du joueur
        if (partie.ScoreCourant <= 0)
        {
            partie.EstMort = true;
            partie.EtatPartie = "Perdue";
            TerminerPartie(partie);
        }
        else
        {
            // Passer à la salle suivante
            partie.IndexSalleCourante++;

            // Si plus de salles → fin
            var nbSalles = partie.Donjon.Salles.Count;
            if (partie.IndexSalleCourante >= nbSalles)
            {
                partie.EtatPartie = "Terminee";
                TerminerPartie(partie);
            }
        }

        await _context.SaveChangesAsync();
        return partie;
    }

    // ---------- Méthodes privées de logique interne ----------

    private Donjon GenererDonjonAleatoire()
    {
        int nbSalles = _random.Next(1, 6); // entre 1 et 5

        var donjon = new Donjon
        {
            Nom = $"Donjon {_random.Next(1000, 9999)}",
            Difficulte = _random.Next(1, 6),
            Salles = new List<Salle>()
        };

        for (int i = 0; i < nbSalles; i++)
        {
            var type = (RoomType)_random.Next(0, Enum.GetValues(typeof(RoomType)).Length);

            var salle = new Salle
            {
                Type = type,
                Description = GenererDescriptionSalle(type),
                Difficulte = _random.Next(1, 6),
                PointsGagnes = _random.Next(5, 16), // 5 à 15
                PointsPerdus = _random.Next(5, 16)
            };

            donjon.Salles.Add(salle);
        }

        return donjon;
    }

    private string GenererDescriptionSalle(RoomType type)
    {
        return type switch
        {
            RoomType.Enemy => "Un terrible monstre bloque le passage.",
            RoomType.Chest => "Un coffre mystérieux se trouve au centre de la salle.",
            RoomType.Trap => "Le sol est couvert de dalles suspectes. Un piège ?",
            RoomType.Puzzle => "Une énigme ancienne est gravée sur le mur.",
            RoomType.Empty => "La salle semble vide, mais est-ce vraiment le cas ?",
            _ => "Une salle inconnue."
        };
    }

    private bool Reussite(double probabilite)
    {
        return _random.NextDouble() <= probabilite;
    }

    private void TerminerPartie(Partie partie)
    {
        partie.EstTerminee = true;
        partie.DateFin = DateTime.Now;
        partie.ScoreFinal = partie.ScoreCourant;
    }
}
