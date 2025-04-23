using Entities;
using Microsoft.EntityFrameworkCore;

namespace Database;

public class AppDbContext : DbContext
{

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }



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

        modelBuilder.Entity<Gardener>(entity =>
        {
            entity.ToTable("Gardener");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Username).HasColumnName("username");
            entity.Property(e => e.Password).HasColumnName("password");
        });
        modelBuilder.Entity<Greenhouse>(entity =>
        {
            entity.ToTable("Greenhouse");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.GardenerId).HasColumnName("gardenerid");

        });
        modelBuilder.Entity<Plant>(entity =>
        {
            entity.ToTable("Plant");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Species).HasColumnName("species");
            entity.Property(e => e.GreenhouseId).HasColumnName("greenhouseid");

        });
        modelBuilder.Entity<Picture>(entity =>
        {
            entity.ToTable("Picture");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TimeStamp).HasColumnName("date");
            entity.Property(e => e.Url).HasColumnName("url");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.PlantId).HasColumnName("plantid");
        });
        modelBuilder.Entity<Sensor>(entity =>
        {
            entity.ToTable("Sensor");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Type).HasColumnName("type");
            entity.Property(e => e.MetricUnit).HasColumnName("metricunit");
            entity.Property(e => e.GreenhouseId).HasColumnName("greenhouseid");
        });
        modelBuilder.Entity<SensorReading>(entity =>
        {
            entity.ToTable("SensorReading");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Value).HasColumnName("value");
            entity.Property(e => e.TimeStamp).HasColumnName("date");
            entity.Property(e => e.ThresholdValue).HasColumnName("threshold");
            entity.Property(e => e.SensorId).HasColumnName("sensorid");
        });
        modelBuilder.Entity<Prediction>(entity =>
        {
            entity.ToTable("Prediction");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.OptimalTemperature).HasColumnName("optimaltemp");
            entity.Property(e => e.OptimalLight).HasColumnName("optimallight");
            entity.Property(e => e.OptimalHumidity).HasColumnName("optimalhumidity");
            entity.Property(e => e.OptimalWaterLevel).HasColumnName("optimalwaterlevel");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.GreenhouseId).HasColumnName("greenhouseid");
            entity.Property(e => e.SensorReadingId).HasColumnName("sensorreadingid");

        });
        modelBuilder.Entity<WaterPump>(entity =>
        {
            entity.ToTable("WaterPump");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.LastWateredTime).HasColumnName("lastwatered");
            entity.Property(e => e.LastWaterAmount).HasColumnName("lastwateramount");
            entity.Property(e => e.AutoWateringEnabled).HasColumnName("autowatering");
            entity.Property(e => e.WaterTankCapacity).HasColumnName("watertankcapacity");
            entity.Property(e => e.WaterLevel).HasColumnName("currentwaterlevel");
            entity.Property(e => e.ThresholdValue).HasColumnName("thresholdvalue");
        });
        modelBuilder.Entity<Log>(entity =>
        {
            entity.ToTable("Log");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Timestamp).HasColumnName("date");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.SensorReadingId).HasColumnName("sensorreadingid");
            entity.Property(e => e.WaterPumpId).HasColumnName("waterpumpid");
            entity.Property(e => e.GreenhouseId).HasColumnName("greenhouseid");

        });
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("Notification");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Type).HasColumnName("type");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.TimeStamp).HasColumnName("date");
            entity.Property(e => e.IsRead).HasColumnName("isread");
            entity.Property(e => e.SensorReadingId).HasColumnName("sensorreadingid");
            entity.Property(e => e.WaterPumpId).HasColumnName("waterpumpid");
        });
        modelBuilder.Entity<NotificationPreferences>(entity =>
        {
            entity.ToTable("NotificationPreferences");
            entity.Property(e => e.GardenerId).HasColumnName("gardenerid");
            entity.Property(e => e.IsEnabled).HasColumnName("isenabled");

        });
    }



}