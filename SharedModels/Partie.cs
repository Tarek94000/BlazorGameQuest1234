namespace SharedModels;

public class Partie
{
    public int Id { get; set; }
    
    // Lien avec le joueur
    public int JoueurId { get; set; }
    public Joueur? Joueur { get; set; }

    // Lien avec le donjon
    public int DonjonId { get; set; }
    public Donjon? Donjon { get; set; }

    public int ScoreFinal { get; set; }
    public DateTime DateDebut { get; set; } = DateTime.Now;
    public DateTime? DateFin { get; set; }

    // Peut être "EnCours", "Terminee", "Perdue"
    public string EtatPartie { get; set; } = "EnCours";
}
