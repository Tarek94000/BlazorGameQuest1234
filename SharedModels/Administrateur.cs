namespace SharedModels;

public class Administrateur
{
    public int Id { get; set; }
    public string Nom { get; set; } = "";
    public string Email { get; set; } = "";
    public string Role { get; set; } = "Administrateur";
}
