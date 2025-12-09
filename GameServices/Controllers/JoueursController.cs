using GameService.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization; 
using SharedModels;

namespace GameServices.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Joueur")]
public class JoueursController : ControllerBase
{
    private readonly GameDbContext _context;

    public JoueursController(GameDbContext context)
    {
        _context = context;
    }

    // GET: api/joueurs
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Joueur>>> GetAll()
    {
        return await _context.Joueurs.ToListAsync();
    }

    // GET: api/joueurs/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Joueur>> GetById(int id)
    {
        var joueur = await _context.Joueurs.FindAsync(id);
        if (joueur == null)
            return NotFound($"Aucun joueur avec l'id {id}");

        return joueur;
    }

    // POST: api/joueurs
    [HttpPost]
    public async Task<ActionResult<Joueur>> Create(Joueur joueur)
    {
        _context.Joueurs.Add(joueur);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = joueur.Id }, joueur);
    }

    // PUT: api/joueurs/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Joueur joueur)
    {
        if (id != joueur.Id)
            return BadRequest("Id incohérent.");

        _context.Entry(joueur).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/joueurs/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var joueur = await _context.Joueurs.FindAsync(id);
        if (joueur == null)
            return NotFound($"Aucun joueur avec l'id {id}");

        _context.Joueurs.Remove(joueur);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // ============================
    //  NOUVEL ENDPOINT V4 : HISTORIQUE DU JOUEUR
    // ============================

    /// <summary>
    /// Retourne l'historique des parties d'un joueur.
    /// GET /api/joueurs/{id}/parties
    /// </summary>
    [HttpGet("{id:int}/parties")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<PartieHistoriqueDto>>> GetHistorique(int id)
    {
        var joueur = await _context.Joueurs.FindAsync(id);
        if (joueur == null)
            return NotFound($"Aucun joueur avec l'id {id}");

        var parties = await _context.Parties
            .Where(p => p.JoueurId == id)
            .Include(p => p.Donjon)
            .OrderByDescending(p => p.DateDebut)
            .ToListAsync();

        var result = parties.Select(p => new PartieHistoriqueDto
        {
            PartieId = p.Id,
            JoueurId = p.JoueurId,
            DonjonNom = p.Donjon?.Nom ?? "",
            ScoreFinal = p.ScoreFinal,
            DateDebut = p.DateDebut,
            DateFin = p.DateFin,
            EtatPartie = p.EtatPartie
        });

        return Ok(result);
    }

    /// <summary>
    /// Retourne l'historique du joueur connecté.
    /// GET /api/joueurs/me/parties
    /// </summary>
    [HttpGet("me/parties")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<PartieHistoriqueDto>>> GetMyHistory()
    {
        var keycloakId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(keycloakId))
            return Unauthorized();

        var joueur = await _context.Joueurs.FirstOrDefaultAsync(j => j.KeycloakId == keycloakId);
        if (joueur == null)
            return NotFound("Profil joueur introuvable.");

        return await GetHistorique(joueur.Id);
    }
}


// ============================
// DTO pour l'historique
// ============================

public class PartieHistoriqueDto
{
    public int PartieId { get; set; }
    public int JoueurId { get; set; }
    public string DonjonNom { get; set; } = "";
    public int ScoreFinal { get; set; }
    public DateTime DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
    public string EtatPartie { get; set; } = "";
}
