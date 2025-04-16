﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities;

public class NotificationPreferences
{
    [Key]
    [ForeignKey(nameof(Gardener))]
    public int GardenerId { get; set; }

    public bool IsEnabled { get; set; }

    public Gardener Gardener { get; set; } = null!;
}