namespace BlazorGame.Client.Models;

public class ClassementEntry
{
    public int JoueurId { get; set; }
    public string Nom { get; set; } = "";
    public int MeilleurScore { get; set; }
    public int NombreParties { get; set; }
}
