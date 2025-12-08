using GameService.Data;
using GameServices.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedModels;

namespace GameServices.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    /// Démarre une nouvelle partie pour un joueur.
    /// Exemple : POST /api/game/start?joueurId=1
    /// </summary>
    [HttpPost("start")]
    public async Task<ActionResult<GameStateDto>> Start([FromQuery] int joueurId)
    {
        // Vérifier si le joueur existe
        var joueur = await _context.Joueurs.FirstOrDefaultAsync(j => j.Id == joueurId);
        if (joueur == null)
        {
            return NotFound($"Aucun joueur avec l'id {joueurId}");
        }

        var partie = await _gameLogicService.DemarrerNouvellePartieAsync(joueurId);
        var salleCourante = await _gameLogicService.ObtenirSalleCouranteAsync(partie.Id);

        var state = MapToGameStateDto(partie, salleCourante);
        return Ok(state);
    }

    /// <summary>
    /// Récupère l'état courant d'une partie.
    /// Exemple : GET /api/game/5
    /// </summary>
    [HttpGet("{partieId:int}")]
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
    public async Task<ActionResult<GameStateDto>> ApplyChoice(int partieId, [FromBody] ChoiceRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Choix))
            return BadRequest("Le champ 'choix' est obligatoire.");

        var partie = await _gameLogicService.AppliquerChoixAsync(partieId, request.Choix);
        if (partie == null)
            return NotFound($"Aucune partie avec l'id {partieId}");

        var salleCourante = await _gameLogicService.ObtenirSalleCouranteAsync(partieId);
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
