using Database;
using Entities;
using LogicInterfaces;
using LogicImplements;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IGardenerInterface, GardenerLogic>();
builder.Services.AddScoped<IGreenhouseInterface, GreenhouseLogic>();
builder.Services.AddScoped<ILogInterface, LogLogic>();
builder.Services.AddScoped<IPictureInterface, PictureLogic>();
builder.Services.AddScoped<IPlantInterface, PlantLogic>();
builder.Services.AddScoped<IPredictionInterface, PredictionLogic>();
builder.Services.AddScoped<ISensorInterface, SensorLogic>();
builder.Services.AddScoped<ISensorReadingInterface, SensorReadingLogic>();
builder.Services.AddScoped<IWaterPumpInterface, WaterPumpLogic>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();