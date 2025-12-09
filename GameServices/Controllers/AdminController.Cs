using System.Text;
using GameService.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using SharedModels;

namespace GameServices.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Joueur")]
public class AdminController : ControllerBase
{
    private readonly GameDbContext _context;

    public AdminController(GameDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Liste tous les joueurs avec quelques infos (admin).
    /// GET /api/admin/joueurs
    /// </summary>
    [HttpGet("joueurs")]
    public async Task<ActionResult<IEnumerable<AdminJoueurDto>>> GetJoueurs()
    {
        var joueurs = await _context.Joueurs
            .Select(j => new AdminJoueurDto
            {
                Id = j.Id,
                Nom = j.Nom,
                EstActif = j.EstActif,
                Score = j.Score,
                NombreParties = _context.Parties.Count(p => p.JoueurId == j.Id)
            })
            .OrderByDescending(j => j.Score)
            .ThenBy(j => j.Nom)
            .ToListAsync();

        return Ok(joueurs);
    }

    /// <summary>
    /// Désactive un joueur (EstActif = false).
    /// PUT /api/admin/joueurs/{id}/desactiver
    /// </summary>
    [HttpPut("joueurs/{id:int}/desactiver")]
    public async Task<IActionResult> DesactiverJoueur(int id)
    {
        var joueur = await _context.Joueurs.FindAsync(id);
        if (joueur == null)
            return NotFound($"Aucun joueur avec l'id {id}");

        joueur.EstActif = false;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Réactive un joueur (EstActif = true).
    /// PUT /api/admin/joueurs/{id}/activer
    /// </summary>
    [HttpPut("joueurs/{id:int}/activer")]
    public async Task<IActionResult> ActiverJoueur(int id)
    {
        var joueur = await _context.Joueurs.FindAsync(id);
        if (joueur == null)
            return NotFound($"Aucun joueur avec l'id {id}");

        joueur.EstActif = true;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Liste toutes les parties enregistrées (admin).
    /// GET /api/admin/parties
    /// </summary>
    [HttpGet("parties")]
    public async Task<ActionResult<IEnumerable<AdminPartieDto>>> GetParties()
    {
        var parties = await _context.Parties
            .Include(p => p.Joueur)
            .Include(p => p.Donjon)
            .OrderByDescending(p => p.DateDebut)
            .ToListAsync();

        var result = parties.Select(p => new AdminPartieDto
        {
            PartieId = p.Id,
            JoueurId = p.JoueurId,
            JoueurNom = p.Joueur?.Nom ?? "",
            DonjonNom = p.Donjon?.Nom ?? "",
            ScoreFinal = p.ScoreFinal,
            DateDebut = p.DateDebut,
            DateFin = p.DateFin,
            EtatPartie = p.EtatPartie
        });

        return Ok(result);
    }

    /// <summary>
    /// Export CSV de la liste des joueurs.
    /// GET /api/admin/export/joueurs
    /// </summary>
    [HttpGet("export/joueurs")]
    public async Task<IActionResult> ExportJoueursCsv()
    {
        var joueurs = await _context.Joueurs
            .OrderBy(j => j.Nom)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("Id;Nom;Score;EstActif;NombreParties");

        foreach (var j in joueurs)
        {
            var nbParties = _context.Parties.Count(p => p.JoueurId == j.Id);
            sb.AppendLine($"{j.Id};{j.Nom};{j.Score};{j.EstActif};{nbParties}");
        }

        var bytes = Encoding.UTF8.GetBytes(sb.ToString());
        var contentType = "text/csv";
        var fileName = $"joueurs_export_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

        return File(bytes, contentType, fileName);
    }
}

// DTOs admin

public class AdminJoueurDto
{
    public int Id { get; set; }
    public string Nom { get; set; } = "";
    public bool EstActif { get; set; }
    public int Score { get; set; }
    public int NombreParties { get; set; }
}

public class AdminPartieDto
{
    public int PartieId { get; set; }
    public int JoueurId { get; set; }
    public string JoueurNom { get; set; } = "";
    public string DonjonNom { get; set; } = "";
    public int ScoreFinal { get; set; }
    public DateTime DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
    public string EtatPartie { get; set; } = "";
}
