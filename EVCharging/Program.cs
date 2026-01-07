using EVCharging.Data;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using EVCharging.Filters;
using EVCharging.Options;
using EVCharging.Services;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Application entry point configuring dependency injection,
/// database context, middleware pipeline, and Swagger documentation.
/// Boots the EV Charging Web API.
/// </summary>

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
        options.Filters.Add<ApiExceptionFilter>();
});

// Registers Razor Pages services so the app can serve server-rendered UI pages.
builder.Services.AddRazorPages();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

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
    var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                $"EV Charging API {description.ApiVersion}");
        }
    });
}

app.UseHttpsRedirection();

// Enables serving static assets (CSS, JS, images) needed by Razor Pages.
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();
// Maps Razor Pages endpoints into the request pipeline.
app.MapRazorPages();

app.Run();
