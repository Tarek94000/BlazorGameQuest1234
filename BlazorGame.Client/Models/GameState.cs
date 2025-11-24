namespace BlazorGame.Client.Models;

public class GameState
{
    public int PartieId { get; set; }
    public int JoueurId { get; set; }
    public int ScoreCourant { get; set; }
    public int ScoreFinal { get; set; }
    public int IndexSalleCourante { get; set; }
    public bool EstTerminee { get; set; }
    public bool EstMort { get; set; }
    public string EtatPartie { get; set; } = "";
    public SalleDto? SalleCourante { get; set; }
}

public class SalleDto
{
    public int Id { get; set; }
    public int Difficulte { get; set; }
    public string Description { get; set; } = "";
    public int PointsGagnes { get; set; }
    public int PointsPerdus { get; set; }

    // ⚠️ IMPORTANT : le backend envoie un int (enum RoomType),
    // donc ici on utilise aussi un int, PAS une string.
    public int Type { get; set; }
}
