using EVCharging.Models;
using EVCharging.Data;
using EVCharging.Dtos;
using Microsoft.EntityFrameworkCore;

namespace EVCharging.Services;

/// <summary>
/// Application service responsible for all charging station-level operations.
/// Encapsulates creation, updates, deletion, and validation logic
/// before committing changes to the database.
/// </summary>
public class ChargingStationService(EvChargingDbContext db, EvValidator validator)
{
    private readonly EvChargingDbContext _db = db;
    private readonly EvValidator _validator = validator;

    public async Task<IEnumerable<ChargingStationResponse>> GetAllAsync()
    {
        var stations = await _db.ChargingStations
            .Include(cs => cs.Connectors)
            .ToListAsync();

        return stations.Select(MapToResponse);
    }

    public async Task<ChargingStationResponse?> GetAsync(Guid id)
    {
        var station = await _db.ChargingStations
            .Include(cs => cs.Connectors)
            .FirstOrDefaultAsync(cs => cs.Id == id);

        return station is not null ? MapToResponse(station) : null;
    }
        
    public async Task<ChargingStationResponse> CreateAsync(CreateChargingStationRequest request)
    {
        var stationId = Guid.NewGuid();

        var station = new ChargingStation
        {
            Id = stationId,
            Name = request.Name,
            GroupId = request.GroupId,
            Connectors = request.Connectors.Select(c => new Connector
            {
                Id = c.Id,
                MaxCurrentAmps = c.MaxCurrentAmps,
                ChargingStationId = stationId
            }).ToList()
        };

        _validator.ValidateStation(station);

        _db.ChargingStations.Add(station);

        await _validator.ValidateGroupCapacityAsync(request.GroupId, station.Connectors, station.Id);

        await _db.SaveChangesAsync();

        return MapToResponse(station);
    }

    public async Task<ChargingStationResponse> UpdateAsync(Guid id, ChargingStationRequest request)
    {
        var station = await _db.ChargingStations.
            Include(cs => cs.Connectors)
            .FirstOrDefaultAsync(cs => cs.Id == id);

        if(station is null) 
            throw new KeyNotFoundException("Charging station not found.");

        if (request.GroupId != station.GroupId)
            throw new InvalidOperationException("Charging station cannot be moved to a different group");

        station.Name = request.Name;
        station.Connectors.Clear();
        station.Connectors = request.Connectors.Select(c => new Connector
        {
            Id = c.Id,
            MaxCurrentAmps = c.MaxCurrentAmps,
            ChargingStationId = station.Id
        }).ToList();

        _validator.ValidateStation(station);

        await _validator.ValidateGroupCapacityAsync(request.GroupId, station.Connectors, station.Id);

        await _db.SaveChangesAsync();

        return MapToResponse(station);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var station = await _db.ChargingStations.FindAsync(id);

        if (station is null) 
            return false;

        _db.ChargingStations.Remove(station);

        await _db.SaveChangesAsync();
        return true;
    }

    private static ChargingStationResponse MapToResponse(ChargingStation station)
    {
        return new ChargingStationResponse
        {
            Id = station.Id,
            Name = station.Name,
            GroupId = station.GroupId,
            Connectors = station.Connectors.Select(c => new ConnectorResponse
            {
                Id = c.Id,
                MaxCurrentAmps = c.MaxCurrentAmps,
                ChargingStationId = station.Id
            }).ToList()
        };
    }
}