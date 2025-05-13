using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities;

public class NotificationPreferences
{
    [Key]
    [ForeignKey(nameof(Gardener))]
    public int GardenerId { get; set; }
    public string Type { get; set; } = null!; // e.g., "Water", "Soil", "Light"
    public bool IsEnabled { get; set; }
}
