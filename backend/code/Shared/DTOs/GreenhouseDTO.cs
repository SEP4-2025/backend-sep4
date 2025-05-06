namespace DTOs;

public class GreenhouseDTO
{
    public string Name { get; set; }
    public int GardenerId { get; set; }

    public bool isEmpty()
    {
        return string.IsNullOrWhiteSpace(Name) || GardenerId == 0;
    }
}