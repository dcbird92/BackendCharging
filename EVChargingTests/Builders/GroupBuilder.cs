using EVCharging.Models;

namespace EVChargingTests.Builders;

/// <summary>
/// Test data builder for creating Group entities with readable, fluent syntax.
/// Ensures tests remain clean, expressive, and isolated from domain construction details.
/// </summary>
public class GroupBuilder
{
    private readonly Group _group = new()
    {
        Id = Guid.NewGuid(),
        Name = "Default Group",
        CapacityAmps = 100,
        ChargingStations = new List<ChargingStation>()
    };

    public GroupBuilder Create(string? name = null)
    {
        var builder = new GroupBuilder();
        if (!string.IsNullOrEmpty(name))
            builder._group.Name = name;

        return builder;
    }

    public GroupBuilder WithId(Guid id)
    {
        _group.Id = id;
        return this;
    }

    public GroupBuilder WithCapacityAmps(int capacityAmps)
    {
        _group.CapacityAmps = capacityAmps;
        return this;
    }

    public GroupBuilder WithName(string name)
    {
        _group.Name = name;
        return this;
    }

    public GroupBuilder WithStation(ChargingStation station)
    {
        station.GroupId = _group.Id;
        _group.ChargingStations.Add(station);
        return this;
    }

    public Group Build() => _group;
}
