using EVCharging.Models;
using EVCharging.Data;
using Microsoft.EntityFrameworkCore;

namespace EVCharging.Services;

/// <summary>
/// Validation component responsible for enforcing
/// Smart Charging business rules such as connector limits,
/// group capacity constraints, and station amperage totals.
/// Used by services prior to persisting changes.
/// </summary>
public class EvValidator
{

    private readonly EvChargingDbContext _db;
    public EvValidator(EvChargingDbContext db)
    {
        _db = db;
    }

    public void ValidateGroup(Group group)
    {
        if(string.IsNullOrWhiteSpace(group.Name))
            throw new InvalidOperationException("Group name is required");

        if(group.CapacityAmps <= 0)
            throw new InvalidOperationException("Group capacity must be > 0");
    }

    public void ValidateGroupCapacity(Group group)
    {
        var totalAmps = group.ChargingStations
            .SelectMany(s => s.Connectors)
            .Sum(c => c.MaxCurrentAmps);

        if (totalAmps > group.CapacityAmps)
            throw new InvalidOperationException($"Total Amps {totalAmps} exceed the group capacity {group.CapacityAmps}");
    }

    public void  ValidateConnector(Connector connector)
    {
        if (connector.Id < 1 || connector.Id > 5)
            throw new InvalidOperationException("Connector ID must be between 1-5");

        if (connector.MaxCurrentAmps <= 0)
            throw new InvalidOperationException("Max amps must be > 0");

        if (connector.ChargingStationId == Guid.Empty)
            throw new InvalidOperationException("Connector must belong to a station");
    }

    public void ValidateStation(ChargingStation chargingStation)
    {
        if(string.IsNullOrWhiteSpace(chargingStation.Name))
            throw new InvalidOperationException("Station name is required");

        if (chargingStation.GroupId == Guid.Empty)
            throw new InvalidOperationException("Station must belong to a group.");

        if (chargingStation.Connectors.Count < 1 || chargingStation.Connectors.Count > 5)
            throw new InvalidOperationException("Station must have between 1-5 connectors");

        var connectorIds = new HashSet<int>();
        foreach(var connector in chargingStation.Connectors)
        {
            ValidateConnector(connector);

            if (!connectorIds.Add(connector.Id))
                throw new InvalidOperationException($"Duplicate connector ID {connector.Id} found in station.");

            if(chargingStation.Id != Guid.Empty && connector.ChargingStationId != chargingStation.Id)
                throw new InvalidOperationException("Connector's ChargingStationId does not match the station's Id.");
        }
    }

    public async Task ValidateGroupCapacityAsync(Guid groupId, IEnumerable<Connector> stagedConnectors, Guid? stationId = null)
    {
        var group = await _db.Groups
            .Include(g => g.ChargingStations)
            .ThenInclude(cs => cs.Connectors)
            .FirstOrDefaultAsync(g => g.Id == groupId);

        if (group is null)
            throw new KeyNotFoundException("Group not found");

        ValidateGroup(group);

        var totalAmps = group.ChargingStations
            .SelectMany(s => s.Connectors)
            .Sum(c => c.MaxCurrentAmps);

        if (stationId.HasValue)
        {
            var existingStation = group.ChargingStations.FirstOrDefault(s => s.Id == stationId.Value);
            if (existingStation is not null)
                totalAmps -= existingStation.Connectors.Sum(c => c.MaxCurrentAmps);
        }

        totalAmps += stagedConnectors.Sum(c => c.MaxCurrentAmps);

        if (totalAmps > group.CapacityAmps)
            throw new InvalidOperationException($"Total Amps {totalAmps} exceed the group capacity {group.CapacityAmps}");
    }
}
