using EVCharging.Dtos;
using EVChargingTests.Utility;
using System.Net;
using System.Net.Http.Json;

namespace EVChargingTests.Controllers;

/// <summary>
/// End-to-end tests for GroupController using real HTTP requests.
/// Confirms routing, DTO binding, validation behavior, CRUD operations,
/// and database persistence through the API pipeline.
/// </summary>
public class GroupControllerTests : IClassFixture<WebAppFactory>
{
    private readonly HttpClient _client;

    public GroupControllerTests(WebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task POST_groups_ShouldCreateGroup()
    {
        var request = new GroupRequest
        {
            Name = "G1",
            CapacityAmps = 100
        };

        var response = await _client.PostAsJsonAsync("/groups", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var group = await response.Content.ReadFromJsonAsync<GroupResponse>();

        Assert.NotNull(group);
        Assert.Equal("G1", group.Name);
    }

    [Fact]
    public async Task GET_groups_id_ShouldReturnGroup()
    {
        var created = await Helper.CreateGroup(_client, "G1", 50);

        Assert.NotNull(created);

        var response = await _client.GetAsync($"/groups/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var group = await response.Content.ReadFromJsonAsync<GroupResponse>();

        Assert.Equal("G1", group.Name);
    }

    [Fact]
    public async Task GET_groups_ShouldReturnAllGroups()
    {
        await Helper.CreateGroup(_client, "G1", 50);
        await Helper.CreateGroup(_client, "G2", 75);

        var response = await _client.GetAsync("/groups");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var list = await response.Content.ReadFromJsonAsync<List<GroupResponse>>();

        Assert.NotNull(list);

        Assert.True(list.Count >= 2);
    }

    [Fact]
    public async Task DELETE_groups_id_ShouldDeleteGroup()
    {
        var create = await _client.PostAsJsonAsync("/groups",
            new GroupRequest { Name = "DeleteMe", CapacityAmps = 50 });

        var created = await create.Content.ReadFromJsonAsync<GroupResponse>();

        Assert.NotNull(created);

        var delete = await _client.DeleteAsync($"/groups/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);

        // Verify deletion
        var get = await _client.GetAsync($"/api/groups/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    }

    [Fact]
    public async Task PUT_groups_id_WithMissingGroup_ShouldReturnNotFound()
    {
        var missingId = Guid.NewGuid();

        var response = await _client.PutAsJsonAsync($"/groups/{missingId}",
            new GroupRequest { Name = "Missing", CapacityAmps = 10 });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DELETE_groups_id_WithMissingGroup_ShouldReturnNotFound()
    {
        var missingId = Guid.NewGuid();

        var response = await _client.DeleteAsync($"/groups/{missingId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
