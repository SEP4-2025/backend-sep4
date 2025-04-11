namespace Entities
{
    public class Log
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }

        public int SensorReadingId { get; set; }
        public int WaterPumpId { get; set; }
        public int GreenhouseId { get; set; }
    }
}
