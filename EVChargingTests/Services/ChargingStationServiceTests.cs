using EVCharging.Dtos;
using EVCharging.Services;
using EVChargingTests.Builders;
using EVChargingTests.Utility;

namespace EVChargingTests.Services;

/// <summary>
/// Unit tests for the ChargingStationService class.
/// Validates business rules, CRUD logic, and ensures
/// the service behaves correctly independent of controllers.
/// </summary>
public class ChargingStationServiceTests
{
    [Fact]
    public async Task CreateStation_ShouldAddConnectors()
    {
        var db = TestDBFactory.Create();
        var validator = new EvValidator(db);

        var group = new GroupBuilder()
                        .Create("G1")
                        .WithCapacityAmps(100)
                        .Build();

        db.Groups.Add(group);
        db.SaveChanges();

        var stationService = new ChargingStationService(db, validator);

        var request = new CreateChargingStationRequest
        {
            Name = "S1",
            GroupId = group.Id,
            Connectors = new List<CreateConnectorRequest>()
            {
                new CreateConnectorRequest { Id = 1, MaxCurrentAmps = 30 },
                new CreateConnectorRequest { Id = 2, MaxCurrentAmps = 20 }
            }
        };

        var result = await stationService.CreateAsync(request);

        Assert.Equal("S1", result.Name);
        Assert.Equal(2, result.Connectors.Count);
    }

    [Fact]
    public async Task CreateStation_ShouldFail_WhenNoConnectorsProvided()
    {
        var db = TestDBFactory.Create();
        var validator = new EvValidator(db);

        var guid = new Guid();

        var group = new GroupBuilder()
                        .Create("G1")
                        .WithId(guid)
                        .WithCapacityAmps(100)
                        .Build();

        db.Groups.Add(group);
        db.SaveChanges();

        var stationService = new ChargingStationService(db, validator);

        var request = new CreateChargingStationRequest
        {
            Name = "S1",
            GroupId = group.Id,
            Connectors = new()
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stationService.CreateAsync(request));
    }

    [Fact]
    public async Task CreateStation_ShouldFail_WhenToManyConnectorsProvided()
    {
        var db = TestDBFactory.Create();
        var validator = new EvValidator(db);

        var guid = new Guid();

        var group = new GroupBuilder()
                        .Create("G1")
                        .WithId(guid)
                        .WithCapacityAmps(100)
                        .Build();

        db.Groups.Add(group);
        db.SaveChanges();

        var stationService = new ChargingStationService(db, validator);

        var request = new CreateChargingStationRequest
        {
            Name = "S1",
            GroupId = group.Id,
            Connectors = new List<CreateConnectorRequest>()
            {
                new CreateConnectorRequest { Id = 1, MaxCurrentAmps = 10 },
                new CreateConnectorRequest { Id = 2, MaxCurrentAmps = 10 },
                new CreateConnectorRequest { Id = 3, MaxCurrentAmps = 10 },
                new CreateConnectorRequest { Id = 4, MaxCurrentAmps = 10 },
                new CreateConnectorRequest { Id = 5, MaxCurrentAmps = 10 },
                new CreateConnectorRequest { Id = 6, MaxCurrentAmps = 10 }
            }
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stationService.CreateAsync(request));
    }
}
