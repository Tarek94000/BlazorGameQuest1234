using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameService.Data;
using SharedModels;

namespace GameService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JoueursController : ControllerBase
{
    private readonly GameDbContext _context;

    public JoueursController(GameDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Joueur>>> GetAll() =>
        await _context.Joueurs.ToListAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<Joueur>> GetById(int id)
    {
        var joueur = await _context.Joueurs.FindAsync(id);
        return joueur == null ? NotFound() : joueur;
    }

    [HttpPost]
    public async Task<ActionResult<Joueur>> Create(Joueur joueur)
    {
        _context.Joueurs.Add(joueur);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = joueur.Id }, joueur);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Joueur joueur)
    {
        if (id != joueur.Id) return BadRequest();
        _context.Entry(joueur).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var joueur = await _context.Joueurs.FindAsync(id);
        if (joueur == null) return NotFound();
        _context.Joueurs.Remove(joueur);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
