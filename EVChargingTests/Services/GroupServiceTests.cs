using EVCharging.Dtos;
using EVCharging.Services;
using EVChargingTests.Builders;
using EVChargingTests.Utility;

namespace EVChargingTests.Services;

/// <summary>
/// Unit tests for the GroupService class.
/// Validates business rules, CRUD logic, and ensures
/// the service behaves correctly independent of controllers.
/// </summary>
public class GroupServiceTests
{
    [Fact]
    public async Task CreateGroup_ShouldSucceed()
    {
        var db = TestDBFactory.Create();
        var validator = new EvValidator(db);
        var groupService = new GroupService(db, validator);

        var request = new GroupRequest
        {
            Name = "Test Group",
            CapacityAmps = 100
        };

        var result = await groupService.CreateAsync(request);
        Assert.NotNull(result);
        Assert.True(result.Id != Guid.Empty);
        Assert.Equal("Test Group", result.Name);
        Assert.Equal(100, result.CapacityAmps);
    }

    [Fact]
    public async Task GetAllGroups_ShouldSucceed()
    {
        var db = TestDBFactory.Create();
        var validator = new EvValidator(db);

        db.Groups.Add(new GroupBuilder().Create().Build());
        db.Groups.Add(new GroupBuilder().Create().Build());
        db.SaveChanges();

        var groupService = new GroupService(db, validator);

        var groups = await groupService.GetAllAsync();

        Assert.Equal(2, groups.Count());
    }

    [Fact]
    public async Task UpdateGroup_ShouldModifyNameAndCapacity()
    {
        var db = TestDBFactory.Create();
        var validator = new EvValidator(db);
        var groupService = new GroupService(db, validator);

        var group = new GroupBuilder()
            .Create()
            .WithName("Original")
            .WithCapacityAmps(100)
            .Build();

        db.Groups.Add(group);
        db.SaveChanges();

        var updateRequest = new GroupRequest
        {
            Name = "Updated",
            CapacityAmps = 200
        };

        var updatedGroup = await groupService.UpdateAsync(group.Id, updateRequest);

        Assert.NotNull(updatedGroup);
        Assert.Equal(group.Id, updatedGroup.Id);
        Assert.Equal("Updated", updatedGroup.Name);
        Assert.Equal(200, updatedGroup.CapacityAmps);
    }

    [Fact]
    public async Task UpdateGroup_ShouldFail_WhenNewCapacityIsTooLow()
    {
        var db = TestDBFactory.Create();
        var validator = new EvValidator(db);
        var groupService = new GroupService(db, validator);

        var guid = Guid.NewGuid();

        var group = new GroupBuilder()
            .Create("G1")
            .WithCapacityAmps(70)
            .WithId(guid)
            .WithStation(
                new ChargingStationBuilder()
                    .Create()
                    .WithGroupId(guid)
                    .WithConnector(1, 40)
                    .WithConnector(2, 30)
                    .Build()
            )
            .Build();

        db.Groups.Add(group);
        db.SaveChanges();

        var request = new GroupRequest
        {
            Name = "G1",
            CapacityAmps = 50 // < 40+30 = 70
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            groupService.UpdateAsync(group.Id, request));
    }
}
