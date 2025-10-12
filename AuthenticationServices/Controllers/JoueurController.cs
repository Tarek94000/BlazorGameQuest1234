using Microsoft.AspNetCore.Mvc;
using SharedModels;

namespace AuthenticationServices.Controllers;

[ApiController]
[Route("[controller]")]
public class JoueurController : ControllerBase
{
    private static readonly List<Joueur> joueurs = new()
    {
        new Joueur { Nom = "Alice", Score = 120 },
        new Joueur { Nom = "Bob", Score = 80 },
    };

    [HttpGet]
    public IEnumerable<Joueur> Get() => joueurs;
}
