namespace SharedModels;

public class Partie
{
    public int Id { get; set; }

    // Joueur lié à la partie
    public int JoueurId { get; set; }
    public Joueur? Joueur { get; set; }

    // Donjon associé à cette partie
    public int DonjonId { get; set; }
    public Donjon? Donjon { get; set; }

    // Score pendant la partie
    public int ScoreCourant { get; set; }

    // Score final (rempli seulement à la fin)
    public int ScoreFinal { get; set; }

    // Index de la salle où se trouve le joueur (0 = première salle)
    public int IndexSalleCourante { get; set; }

    // Indique si la partie est terminée (victoire, défaite, toutes les salles faites)
    public bool EstTerminee { get; set; }

    // Optionnel : permet de marquer si le joueur est mort
    public bool EstMort { get; set; }

    // Suivi de temps
    public DateTime DateDebut { get; set; } = DateTime.Now;
    public DateTime? DateFin { get; set; }

    // Etat textuel ("EnCours", "Terminee", "Perdue", etc.)
    public string EtatPartie { get; set; } = "EnCours";
}
