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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Plant API", Version = "v1" });
    c.OperationFilter<FileUploadOperationFilter>();
});


// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }
    );
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

// Configure default gardner in the database if doesnt exist yet
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();

