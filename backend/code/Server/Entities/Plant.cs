namespace Entities;

public class Plant
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Species { get; set; }
    public List<Picture> Pictures { get; set; } = new List<Picture>();

    public int GreenhouseId { get; set; }
}
