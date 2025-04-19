namespace Entities;

public class NotificationPreferences
{
    public int GardenerId { get; set; }
    public string Type { get; set; } // e.g., "Water", "Soil", "Light"
    public bool IsEnabled { get; set; }
}