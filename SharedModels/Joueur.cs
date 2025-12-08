namespace SharedModels;

public class Joueur
{
<<<<<<< HEAD
    public int Id { get; set; }
    public string Nom { get; set; } = "";
    public int Score { get; set; } = 0;
    public bool EstActif { get; set; } = true;
    public string KeycloakId { get; set; } = "";

    // Historique des parties
    public List<Partie>? HistoriqueParties { get; set; }
=======
    public string Nom { get; set; } = "";
    public int Score { get; set; } = 0;
>>>>>>> origin/prod
}
