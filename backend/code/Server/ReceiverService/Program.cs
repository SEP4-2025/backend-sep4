using Database;
using DTOs;
using LogicImplements;
using LogicInterfaces;
using Microsoft.EntityFrameworkCore;

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
            builder.Services.AddHttpClient();

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            builder.Services.AddHostedService<SensorReceiverService>();

            builder.Services.AddHealthChecks().AddCheck<SensorReceiverService>("mqtt_connection");
            
            builder.Configuration
                .AddJsonFile("receiverSettings.json", optional: false)
                .AddJsonFile($"receiverSettings.{builder.Environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            var app = builder.Build();

            app.MapHealthChecks("/health");

            await app.RunAsync();
        }
    }
}
