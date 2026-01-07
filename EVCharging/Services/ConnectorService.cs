using EVCharging.Data;
using EVCharging.Dtos;
using EVCharging.Models;
using Microsoft.EntityFrameworkCore;

namespace EVCharging.Services;

/// <summary>
/// Application service responsible for all connector-level operations.
/// Encapsulates creation, updates, deletion, and validation logic
/// before committing changes to the database.
/// </summary>
public class ConnectorService(EvChargingDbContext db, EvValidator validator)
{ 
    private readonly EvChargingDbContext _db = db;
    private readonly EvValidator _validator = validator;

    public async Task<IEnumerable<ConnectorResponse>> GetAllAsync()
    {
        var connectors =  await _db.Connectors.ToListAsync();
        return connectors.Select(MapToResponse);
    }

    public async Task<IEnumerable<ConnectorResponse>> GetByStationAsync(Guid stationId)
    {
        var station = await GetStationWithConnectorsAsync(stationId);

        return station.Connectors.Select(MapToResponse);
    }

    public async Task<ConnectorResponse?> GetAsync(Guid stationId, int connectorId)
    {
        var connector = await _db.Connectors
            .FirstOrDefaultAsync(c => c.ChargingStationId == stationId && c.Id == connectorId);

        return connector is not null ? MapToResponse(connector) : null;
    }

    public async Task<ConnectorResponse> CreateAsync(ConnectorRequest request)
    {
        var station = await GetStationWithConnectorsAsync(request.ChargingStationId);

        if (station.Connectors.Any(c => c.Id == request.Id))
            throw new InvalidOperationException("Connector with the same ID already exists in this station.");

        if (station.Connectors.Count >= 5)
            throw new InvalidOperationException("Station must have between 1-5 connectors");

        var connector = new Connector
        {
            Id = request.Id,
            MaxCurrentAmps = request.MaxCurrentAmps,
            ChargingStationId = request.ChargingStationId
        };

        _validator.ValidateConnector(connector);

        station.Connectors.Add(connector);

        _validator.ValidateStation(station);

        await _validator.ValidateGroupCapacityAsync(station.GroupId, station.Connectors, station.Id);

        await _db.SaveChangesAsync();

        return MapToResponse(connector);
    }

    public async Task<ConnectorResponse> UpdateAsync(Guid stationId, int connectorId, ConnectorRequest request)
    {
        var station = await GetStationWithConnectorsAsync(stationId);

        var connector = station.Connectors.FirstOrDefault(c => c.Id == connectorId);

        if (connector is null)
            throw new KeyNotFoundException("Connector not found.");

        connector.MaxCurrentAmps = request.MaxCurrentAmps;

        _validator.ValidateConnector(connector);

        await _validator.ValidateGroupCapacityAsync(station.GroupId, station.Connectors, station.Id);

        await _db.SaveChangesAsync();

        return MapToResponse(connector);
    }

    public async Task<bool> DeleteAsync(Guid stationId, int connectorId)
    {
        var station = await GetStationWithConnectorsAsync(stationId);

        if (station.Connectors.Count <= 1)
            throw new InvalidOperationException("A station must have atleast one connector");
            
        var connector = station.Connectors.FirstOrDefault(c => c.Id == connectorId);

        if (connector is null)
            return false;

        _db.Connectors.Remove(connector);
        await _db.SaveChangesAsync();
        return true;
    }

    private static ConnectorResponse MapToResponse(Connector connector)
    {
        return new ConnectorResponse
        {
            Id = connector.Id,
            MaxCurrentAmps = connector.MaxCurrentAmps,
            ChargingStationId = connector.ChargingStationId
        };
    }

    private async Task<ChargingStation> GetStationWithConnectorsAsync(Guid stationId)
    {
        var station = await _db.ChargingStations
            .Include(s => s.Connectors)
            .FirstOrDefaultAsync(s => s.Id == stationId);

        if (station is null)
            throw new KeyNotFoundException("Charging station not found.");

        return station;
    }
}
