namespace BlazorGame.Client.Models;

public class AdminJoueur
{
    public int Id { get; set; }
    public string Nom { get; set; } = "";
    public bool EstActif { get; set; }
    public int Score { get; set; }
    public int NombreParties { get; set; }
}
