namespace SharedModels;

public class Room
{
    public int Id { get; set; }
    public RoomType Type { get; set; }
    public string Description { get; set; } = "";
}
