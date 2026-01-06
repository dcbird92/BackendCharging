using EVCharging.Dtos;
using EVCharging.Services;
using EVChargingTests.Builders;
using EVChargingTests.Utility;

namespace EVChargingTests.Services;

/// <summary>
/// Unit tests for the ConnectorService class.
/// Validates business rules, CRUD logic, and ensures
/// the service behaves correctly independent of controllers.
/// </summary>
public class ConnectorServiceTests
{
    [Fact]
    public async Task CreateConnector_ShouldFail_WhenDupicateId()
    {
        var db = TestDBFactory.Create();
        var validator = new EvValidator(db);

        var id = Guid.NewGuid();

        var station = new ChargingStationBuilder()
                        .Create("Station1")
                        .WithGroupId(id)
                        .WithConnector(1, 10)
                        .Build();

        var group = new GroupBuilder()
                        .Create("A")
                        .WithCapacityAmps(100)
                        .WithId(id)
                        .WithStation(station)
                        .Build();

        db.Groups.Add(group);
        db.SaveChanges();

        var service = new ConnectorService(db, validator);

        var request = new ConnectorRequest
        {
            Id = 1,
            MaxCurrentAmps = 20,
            ChargingStationId = station.Id
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateAsync(request));
    }

    [Fact]
    public async Task UpdateConnector_ShouldFail_IncreaseAmpBeyondCapacity()
    {
        var db = TestDBFactory.Create();
        var validator = new EvValidator(db);
        var id = Guid.NewGuid();

        var station = new ChargingStationBuilder()
                        .Create("S1")
                        .WithGroupId(id)
                        .WithConnector(1, 20)
                        .Build();

        var group = new GroupBuilder()
                        .Create("A")
                        .WithCapacityAmps(30)
                        .WithId(id)
                        .WithStation(station)
                        .Build();

        db.Groups.Add(group);
        db.SaveChanges();

        var connectorService = new ConnectorService(db, validator);

        var request = new ConnectorRequest
        {
            Id = 1,
            MaxCurrentAmps = 50,
            ChargingStationId = station.Id
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            connectorService.UpdateAsync(station.Id, 1, request));

    }
}
