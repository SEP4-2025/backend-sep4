namespace DTOs;

public class PlantDTO
{
    public string Name { get; set; }
    public string Species { get; set; }
    public int GreenhouseId { get; set; }
    
    public bool IsEmpty()
    {
        return string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Species) || GreenhouseId <= 0;
    }
}