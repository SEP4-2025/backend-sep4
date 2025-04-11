namespace Entities;

public class Picture
{
    public int Id { get; set; }
    public string Url { get; set; }
    public string Note { get; set; }
    public DateTime TimeStamp { get; set; }
    
    public int PlantId { get; set; }
}
