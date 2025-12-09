using GameService.Data;
using GameServices.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;  
using SharedModels;

namespace GameServices.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Joueur")]
public class GameController : ControllerBase
{
    private readonly GameLogicService _gameLogicService;
    private readonly GameDbContext _context;

    public GameController(GameLogicService gameLogicService, GameDbContext context)
    {
        _gameLogicService = gameLogicService;
        _context = context;
    }

    /// <summary>
    /// Démarre une nouvelle partie.
    /// Si l'utilisateur est connecté via Keycloak, on utilise son ID pour récupérer/créer le Joueur.
    /// Sinon on peut passer ?joueurId=... (mode compatibilité/invité).
    /// </summary>
    [HttpPost("start")]
    [AllowAnonymous]
    public async Task<ActionResult<GameStateDto>> Start([FromQuery] int? joueurId)
    {
        Joueur? joueur = null;

        // 1. Essayer de récupérer l'ID Keycloak depuis le token (NameIdentifier = 'sub')
        // Grâce à l'attribut [Authorize], si un token est présent, User.Identity sera renseigné.
        var keycloakId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(keycloakId))
        {
            // C'est un utilisateur authentifié via Keycloak
            // On cherche s'il existe déjà dans notre base de données locale
            joueur = await _context.Joueurs.FirstOrDefaultAsync(j => j.KeycloakId == keycloakId);

            if (joueur == null)
            {
                // Premier lancement pour cet utilisateur : on crée le profil Joueur automatiquement
                // Cela évite d'avoir une étape d'inscription séparée.
                var name = User.Identity?.Name ?? "Aventurier";
                joueur = new Joueur 
                { 
                    Nom = name, 
                    KeycloakId = keycloakId, 
                    Score = 0, 
                    EstActif = true 
                };
                _context.Joueurs.Add(joueur);
                await _context.SaveChangesAsync();
            }
        }
        else if (joueurId.HasValue)
        {
            // Mode manuel / Invité (si autorisé sans token, ou token sans claim)
            // Permet de tester ou de jouer sans compte.
            joueur = await _context.Joueurs.FirstOrDefaultAsync(j => j.Id == joueurId);
        }

        if (joueur == null)
        {
            return BadRequest("Impossible d'identifier le joueur. Connectez-vous ou fournissez un ID.");
        }

        // 2. Démarrer la partie avec l'ID du joueur trouvé
        var partie = await _gameLogicService.DemarrerNouvellePartieAsync(joueur.Id);
        var salleCourante = await _gameLogicService.ObtenirSalleCouranteAsync(partie.Id);

        var state = MapToGameStateDto(partie, salleCourante);
        return Ok(state);
    }

    /// <summary>
    /// Récupère l'état courant d'une partie.
    /// Exemple : GET /api/game/5
    /// </summary>
    [HttpGet("{partieId:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<GameStateDto>> GetState(int partieId)
    {
        var partie = await _context.Parties
            .Include(p => p.Donjon)
            .FirstOrDefaultAsync(p => p.Id == partieId);

        if (partie == null)
            return NotFound($"Aucune partie avec l'id {partieId}");

        var salleCourante = await _gameLogicService.ObtenirSalleCouranteAsync(partieId);
        var state = MapToGameStateDto(partie, salleCourante);
        return Ok(state);
    }

    /// <summary>
    /// Applique un choix du joueur dans la salle courante.
    /// Exemple : POST /api/game/5/choice   body: { "choix": "Combattre" }
    /// </summary>
    [HttpPost("{partieId:int}/choice")]
    [AllowAnonymous]
    public async Task<ActionResult<GameStateDto>> ApplyChoice(int partieId, [FromBody] ChoiceRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Choix))
            return BadRequest("Le champ 'choix' est obligatoire.");

        // Appel au service de logique métier (GameLogicService) qui contient les règles du jeu.
        // Ce service va modifier l'état de la partie en base de données.
        var partie = await _gameLogicService.AppliquerChoixAsync(partieId, request.Choix);
        if (partie == null)
            return NotFound($"Aucune partie avec l'id {partieId}");

        // On récupère la nouvelle salle courante pour l'envoyer au client.
        var salleCourante = await _gameLogicService.ObtenirSalleCouranteAsync(partieId);
        
        // Transformation de l'entité Partie en DTO (Data Transfer Object) pour le client
        var state = MapToGameStateDto(partie, salleCourante);
        return Ok(state);
    }

    // --------- Méthode interne pour construire la réponse envoyée au client ---------

    private static GameStateDto MapToGameStateDto(Partie partie, Salle? salleCourante)
    {
        return new GameStateDto
        {
            PartieId = partie.Id,
            JoueurId = partie.JoueurId,
            ScoreCourant = partie.ScoreCourant,
            ScoreFinal = partie.ScoreFinal,
            IndexSalleCourante = partie.IndexSalleCourante,
            EstTerminee = partie.EstTerminee,
            EstMort = partie.EstMort,
            EtatPartie = partie.EtatPartie,
            SalleCourante = salleCourante
        };
    }
}


// ============================
// DTOs utilisés par le frontend
// ============================

/// <summary>
/// DTO renvoyé au front pour représenter l'état d'une partie.
/// </summary>
public class GameStateDto
{
    public int PartieId { get; set; }
    public int JoueurId { get; set; }
    public int ScoreCourant { get; set; }
    public int ScoreFinal { get; set; }
    public int IndexSalleCourante { get; set; }
    public bool EstTerminee { get; set; }
    public bool EstMort { get; set; }
    public string EtatPartie { get; set; } = "";
    public Salle? SalleCourante { get; set; }
}

/// <summary>
/// Requête envoyée par le joueur lorsqu'il fait un choix dans une salle.
/// Exemple JSON: { "choix": "Combattre" }
/// </summary>
public class ChoiceRequest
{
    public string Choix { get; set; } = "";
}
