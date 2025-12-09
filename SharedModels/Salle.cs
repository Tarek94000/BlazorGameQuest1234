namespace SharedModels;

public class Salle
{
    public int Id { get; set; }
    public RoomType Type { get; set; }
    public string Description { get; set; } = "";
    public int Difficulte { get; set; }
    public int PointsGagnes { get; set; }
    public int PointsPerdus { get; set; }

    // Relation avec le donjon
    public int DonjonId { get; set; }
}
