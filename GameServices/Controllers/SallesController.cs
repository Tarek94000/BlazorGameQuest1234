using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameService.Data;
using SharedModels;

namespace GameService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SallesController : ControllerBase
{
    private readonly GameDbContext _context;

    public SallesController(GameDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Salle>>> GetAll() =>
        await _context.Salles.ToListAsync();

    [HttpPost]
    public async Task<ActionResult<Salle>> Create(Salle salle)
    {
        _context.Salles.Add(salle);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = salle.Id }, salle);
    }
}
