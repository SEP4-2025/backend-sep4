using System.Text;
using Database;
using Entities;
using DotNetEnv;
using LogicImplements;
using LogicInterfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ReceiverService;
using Tools;
using WebAPI.Services;


var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAllOrigins",
        policyBuilder =>
        {
            policyBuilder
                .WithOrigins("http://localhost:5173", "https://sep4-2025.github.io/frontend-sep4")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
    );
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

//Create Google Cloud Storage Credentials
// Attempt to load .env from current directory (Docker case)
if (File.Exists(".env"))
{
    Console.WriteLine("Loading .env from current directory.");
    Env.Load();
}
else
{
    // Fallback for local development: ../../.env relative to Server/WebAPI/
    var fallbackEnvPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\.env"));
    Console.WriteLine($"Fallback: trying {fallbackEnvPath}");
    if (File.Exists(fallbackEnvPath))
    {
        Env.Load(fallbackEnvPath);
    }
    else
    {
        Console.WriteLine("No .env file found!");
    }
}

// Validate GCS_KEY_JSON environment variable
var b64 = Environment.GetEnvironmentVariable("GCS_KEY_JSON");
if (string.IsNullOrWhiteSpace(b64))
{
    Console.WriteLine("GCS_KEY_JSON missing or empty");
    return;
}
else
{
    Console.WriteLine($"GCS_KEY_JSON loaded with length: {b64.Length}");
}

// Write Google Cloud credentials file
var keyPath = "/tmp/gcs-key.json";

File.WriteAllBytes(keyPath, Convert.FromBase64String(b64));
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyPath);

Console.WriteLine($"GCS credentials written to: {keyPath}");
builder.Services.AddScoped<IGardenerInterface, GardenerLogic>();
builder.Services.AddScoped<IGreenhouseInterface, GreenhouseLogic>();
builder.Services.AddScoped<ILogInterface, LogLogic>();
builder.Services.AddScoped<IPictureInterface, PictureLogic>();
builder.Services.AddScoped<IPlantInterface, PlantLogic>();
builder.Services.AddScoped<IPredictionInterface, PredictionLogic>();
builder.Services.AddScoped<ISensorInterface, SensorLogic>();
builder.Services.AddScoped<ISensorReadingInterface, SensorReadingLogic>();
builder.Services.AddScoped<IWaterPumpInterface, WaterPumpLogic>();
builder.Services.AddSingleton<INotificationService, NotificationService>();
builder.Services.AddScoped<INotificationPrefInterface, NotificationPrefLogic>();
builder.Services.AddScoped<INotificationInterface, NotificationLogic>();

// Add HttpClient support
builder.Services.AddHttpClient();

// Add MQTT services
builder.Services.AddSingleton<SensorReceiverService>();
builder.Services.AddSingleton<IWateringService, SensorReceiverService>();

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
            ),
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure default gardner in the database if does not exist yet
using var scope = app.Services.CreateScope();
var DbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

if (!DbContext.Gardeners.Any(g => g.Username == "admin"))
{
    var passwordHasher = new PasswordHasher<Gardener>();

    var gardener = new Gardener
    {
        Username = "admin",
        Password = passwordHasher.HashPassword(new Gardener(), "admin"),
    };
    DbContext.Gardeners.Add(gardener);
    DbContext.SaveChanges();
}

//Add Logger
Logger.Initialize(DbContext);


// Configure the HTTP request pipeline. We might need to adjust for it or get other solution for running local(dev) / cloud(prod)
if (app.Environment.IsDevelopment())
{ //Development mode
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "GrowMate API v1 (dev env)");
        c.RoutePrefix = string.Empty;
    });

    app.UseCors("AllowAllOrigins");
}
else
{
    // Production mode
    app.UseHttpsRedirection();
    app.UseCors("AllowAllOrigins");
}
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/notificationHub");

app.Run();
