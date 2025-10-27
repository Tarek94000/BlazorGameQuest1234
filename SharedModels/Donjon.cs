namespace SharedModels;

public class Donjon
{
    public int Id { get; set; }
    public string Nom { get; set; } = "";
    public int Difficulte { get; set; }

    // Relation : un donjon contient plusieurs salles
    public List<Salle> Salles { get; set; } = new();
}
