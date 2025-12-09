using GameService.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameServices.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClassementController : ControllerBase
{
    private readonly GameDbContext _context;

    public ClassementController(GameDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retourne le classement général des joueurs (Top 10).
    /// MeilleurScore = meilleur score final parmi leurs parties.
    /// GET /api/classement
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClassementEntryDto>>> GetClassement()
    {
        // Requête LINQ pour construire le classement :
        // 1. Pour chaque joueur, on calcule son meilleur score.
        // 2. On trie par score décroissant.
        // 3. On prend les 10 premiers.
        var entries = await _context.Joueurs
            .Select(j => new ClassementEntryDto
            {
                JoueurId = j.Id,
                Nom = j.Nom,
                // Utilisation de Max() sur la table Parties liée pour trouver le record personnel
                MeilleurScore = _context.Parties
                    .Where(p => p.JoueurId == j.Id)
                    .Max(p => (int?)p.ScoreFinal) ?? 0,
                NombreParties = _context.Parties
                    .Count(p => p.JoueurId == j.Id)
            })
            .OrderByDescending(e => e.MeilleurScore)
            .ThenBy(e => e.Nom) // En cas d'égalité, tri alphabétique
            .Take(10)
            .ToListAsync();

        return Ok(entries);
    }
}

/// <summary>
/// DTO d'une entrée de classement.
/// </summary>
public class ClassementEntryDto
{
    public int JoueurId { get; set; }
    public string Nom { get; set; } = "";
    public int MeilleurScore { get; set; }
    public int NombreParties { get; set; }
}
