using EVCharging.Models;

namespace EVChargingTests.Builders;

/// <summary>
/// Test data builder for creating Connector entities with readable, fluent syntax.
/// Ensures tests remain clean, expressive, and isolated from domain construction details.
/// </summary>
public class ConnectorBuilder
{
    private readonly Connector _connector = new()
    {
        Id = 1,
        MaxCurrentAmps = 32,
        ChargingStationId = Guid.NewGuid()
    };
    public ConnectorBuilder Create(int id = 1)
    {
        var builder = new ConnectorBuilder();
        builder._connector.Id = id;
        return builder;
    }
    public ConnectorBuilder WithId(int id)
    {
        _connector.Id = id;
        return this;
    }
    public ConnectorBuilder WithMaxCurrentAmps(int amps)
    {
        _connector.MaxCurrentAmps = amps;
        return this;
    }
    public ConnectorBuilder WithChargingStationId(Guid stationId)
    {
        _connector.ChargingStationId = stationId;
        return this;
    }
    public Connector Build() => _connector;
}
