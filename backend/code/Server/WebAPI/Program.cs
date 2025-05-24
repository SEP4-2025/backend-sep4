using System.Text;
using Database;
using Entities;
using LogicImplements;
using LogicInterfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ReceiverService;
using Tools;
using WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Define JWT Bearer auth scheme
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token.\n\nExample: Bearer eyJhbGciOi..."
    });

    // Apply security requirement globally
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddSignalR();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAllOrigins",
        policyBuilder =>
        {
            policyBuilder
                .WithOrigins("http://localhost:5173", "https://sep4-2025.github.io")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

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

builder.Services.AddHttpClient("ml-api", client =>
{
    client.BaseAddress = new Uri("https://ml-model-service-68779328892.europe-north2.run.app");
    client.Timeout = TimeSpan.FromMinutes(5);
});

var app = builder.Build();


using var scope = app.Services.CreateScope();
var DbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();


//Add Logger
Logger.Initialize(DbContext);

// Configure the HTTP request pipeline. We might need to adjust for it or get other solution for running local(dev) / cloud(prod)
// Apply CORS before other middleware
app.UseCors("AllowAllOrigins");

if (app.Environment.IsDevelopment())
{ //Development mode
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "GrowMate API v1 (dev env)");
    });
}
else
{
    // Production mode
    app.UseHttpsRedirection();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "GrowMate API v1");
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/notificationHub");

app.Run();
