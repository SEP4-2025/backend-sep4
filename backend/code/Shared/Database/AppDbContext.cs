using Entities;
using Microsoft.EntityFrameworkCore;

namespace Database;

public class AppDbContext : DbContext
{

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    

    public DbSet<Gardener> Gardeners => Set<Gardener>();
    public DbSet<Greenhouse> Greenhouses => Set<Greenhouse>();
    public DbSet<Log> Logs => Set<Log>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationPreferences> NotificationPreferences => Set<NotificationPreferences>();
    public DbSet<Picture> Pictures => Set<Picture>();
    public DbSet<Plant> Plants => Set<Plant>();
    public DbSet<Prediction> Predictions => Set<Prediction>();
    public DbSet<Sensor> Sensors => Set<Sensor>();
    public DbSet<SensorReading> SensorReadings => Set<SensorReading>();
    public DbSet<WaterPump> WaterPumps => Set<WaterPump>();




    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("GrowMate");

        modelBuilder.Entity<Gardener>().ToTable("Gardener");
        modelBuilder.Entity<Greenhouse>().ToTable("Greenhouse");
        modelBuilder.Entity<Plant>().ToTable("Plant");
        modelBuilder.Entity<Picture>().ToTable("Picture");
        modelBuilder.Entity<Sensor>().ToTable("Sensor");
        modelBuilder.Entity<SensorReading>().ToTable("SensorReading");
        modelBuilder.Entity<Prediction>().ToTable("Prediction");
        modelBuilder.Entity<WaterPump>().ToTable("WaterPump");
        modelBuilder.Entity<Log>().ToTable("Log");
        modelBuilder.Entity<Notification>().ToTable("Notification");
        modelBuilder.Entity<NotificationPreferences>().ToTable("NotificationPreference");
    }
    
    
   
}