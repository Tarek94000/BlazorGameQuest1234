using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameService.Data;
using SharedModels;

namespace GameService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DonjonsController : ControllerBase
{
    private readonly GameDbContext _context;

    public DonjonsController(GameDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Donjon>>> GetAll() =>
        await _context.Donjons.Include(d => d.Salles).ToListAsync();

    [HttpPost]
    public async Task<ActionResult<Donjon>> Create(Donjon donjon)
    {
        _context.Donjons.Add(donjon);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = donjon.Id }, donjon);
    }
}
