using EVCharging.Models;

namespace EVChargingTests.Builders;

/// <summary>
/// Test data builder for creating ChargingStation entities with readable, fluent syntax.
/// Ensures tests remain clean, expressive, and isolated from domain construction details.
/// </summary>
public class ChargingStationBuilder
{
    private readonly ChargingStation _station = new()
    {
        Id = Guid.NewGuid(),
        Name = "Default Station",
        GroupId = Guid.NewGuid(),
        Connectors = new List<Connector>()
    };

    public ChargingStationBuilder Create(string? name = null)
    {
        var builder = new ChargingStationBuilder();
        if (!string.IsNullOrEmpty(name))
            builder._station.Name = name;
        return builder;
    }
    public ChargingStationBuilder WithId(Guid id)
    {
        _station.Id = id;
        return this;
    }
    public ChargingStationBuilder WithName(string name)
    {
        _station.Name = name;
        return this;
    }

    public ChargingStationBuilder WithGroupId(Guid groupId)
    {
        _station.GroupId = groupId;
        return this;
    }
    public ChargingStationBuilder WithConnector(int id, int amps)
    {
        _station.Connectors.Add(new Connector
        {
            Id = id,
            MaxCurrentAmps = amps,
            ChargingStationId = _station.Id
        });
        return this;
    }
    public ChargingStation Build() => _station;
}
