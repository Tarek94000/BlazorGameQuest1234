namespace BlazorGame.Client.Models;

public class HistoriquePartie
{
    public int PartieId { get; set; }
    public int JoueurId { get; set; }
    public string DonjonNom { get; set; } = "";
    public int ScoreFinal { get; set; }
    public DateTime DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
    public string EtatPartie { get; set; } = "";
}
