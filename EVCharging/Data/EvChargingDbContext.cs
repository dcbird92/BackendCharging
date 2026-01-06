using EVCharging.Models;
using Microsoft.EntityFrameworkCore;

namespace EVCharging.Data;

/// <summary>
/// Entity Framework Core DbContext for the EV Charging domain.
/// Configures entity relationships, cascade behavior, composite keys,
/// and provides access to Groups, ChargingStations, and Connectors.
/// </summary>
public class EvChargingDbContext : DbContext
{
    public EvChargingDbContext(DbContextOptions<EvChargingDbContext> options) : base(options)
    {
    }
    
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<ChargingStation> ChargingStations => Set<ChargingStation>();
    public DbSet<Connector> Connectors => Set<Connector>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Group>()
            .HasMany(g => g.ChargingStations)
            .WithOne(cs => cs.Group)
            .HasForeignKey(cs => cs.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ChargingStation>()
            .HasMany(cs => cs.Connectors)
            .WithOne(c => c.ChargingStation)
            .HasForeignKey(c => c.ChargingStationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Connector ID must be unique within each ChargingStation
        modelBuilder.Entity<Connector>()
            .HasKey(c => new { c.Id, c.ChargingStationId });

        // Connector ID must be 1–5
        modelBuilder.Entity<Connector>()
            .Property(c => c.Id)
            .HasConversion<int>();
    }

}