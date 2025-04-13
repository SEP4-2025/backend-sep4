namespace ReceiverService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting MQTT Receiver Service...");

            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            builder.Services.AddHostedService<SensorReceiverService>();

            builder.Services.AddHealthChecks().AddCheck<SensorReceiverService>("mqtt_connection");

            var app = builder.Build();

            app.MapHealthChecks("/health");

            await app.RunAsync();
        }
    }
}
