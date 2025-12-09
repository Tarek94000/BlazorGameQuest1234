using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameService.Data;
using SharedModels;

namespace GameService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PartiesController : ControllerBase
{
    private readonly GameDbContext _context;

    public PartiesController(GameDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Partie>>> GetAll() =>
        await _context.Parties.Include(p => p.Joueur).Include(p => p.Donjon).ToListAsync();

    [HttpPost]
    public async Task<ActionResult<Partie>> Create(Partie partie)
    {
        _context.Parties.Add(partie);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = partie.Id }, partie);
    }
}
