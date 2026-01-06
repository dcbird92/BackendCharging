using EVCharging.Data;
using EVCharging.Services;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Application entry point configuring dependency injection,
/// database context, middleware pipeline, and Swagger documentation.
/// Boots the EV Charging Web API.
/// </summary>

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext - InMemory
builder.Services.AddDbContext<EvChargingDbContext>(opt =>
    opt.UseInMemoryDatabase("EVChargingDb"));

builder.Services.AddScoped<GroupService>();
builder.Services.AddScoped<ChargingStationService>();
builder.Services.AddScoped<ConnectorService>();

builder.Services.AddScoped<EvValidator>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
