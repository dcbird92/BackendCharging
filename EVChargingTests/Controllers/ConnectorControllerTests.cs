using EVCharging.Dtos;
using EVChargingTests.Utility;
using System.Net;
using System.Net.Http.Json;

namespace EVChargingTests.Controllers;

/// <summary>
/// End-to-end tests for ConnectorController using real HTTP requests.
/// Confirms routing, DTO binding, validation behavior, CRUD operations,
/// and database persistence through the API pipeline.
/// </summary>
public class ConnectorControllerTests : IClassFixture<WebAppFactory>
{
    private readonly HttpClient _client;

    public ConnectorControllerTests(WebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task POST_connectors_ShouldCreateConnector()
    {
        var group = await Helper.CreateGroup(_client);
        var station = await Helper.CreateStation(_client, group.Id);

        var request = new ConnectorRequest
        {
            Id = 2,
            MaxCurrentAmps = 25,
            ChargingStationId = station.Id
        };

        var response = await _client.PostAsJsonAsync(
            $"/stations/{station.Id}/connectors", request);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ConnectorResponse>();

        Assert.Equal(2, result!.Id);
        Assert.Equal(25, result.MaxCurrentAmps);
    }

    [Fact]
    public async Task GET_connectors_ShouldGetConnector()
    {
        var group = await Helper.CreateGroup(_client);
        var station = await Helper.CreateStation(_client, group.Id);

        var response = await _client.GetAsync(
            $"/stations/{station.Id}/connectors/1");

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ConnectorResponse>();

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(10, result.MaxCurrentAmps);
    }

    [Fact]
    public async Task PUT_connectors_ShouldUpdateConnector()
    {
        var group = await Helper.CreateGroup(_client);
        var station = await Helper.CreateStation(_client, group.Id);

        var update = new ConnectorRequest
        {
            Id = 1,
            MaxCurrentAmps = 40,
            ChargingStationId = station.Id
        };

        var response = await _client.PutAsJsonAsync(
            $"/stations/{station.Id}/connectors/1", update);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ConnectorResponse>();

        Assert.NotNull(result);
        Assert.Equal(40, result.MaxCurrentAmps);
    }

    [Fact]
    public async Task DELETE_connectors_ShouldFail_RemoveLastConnector()
    {
        var group = await Helper.CreateGroup(_client);
        var station = await Helper.CreateStation(_client, group.Id);

        var delete = await _client.DeleteAsync($"/stations/{station.Id}/connectors/1");

        Assert.Equal(HttpStatusCode.BadRequest, delete.StatusCode);
    }

    [Fact]
    public async Task PUT_connectors_WithMissingConnector_ShouldReturnNotFound()
    {
        var group = await Helper.CreateGroup(_client);
        var station = await Helper.CreateStation(_client, group.Id);

        var update = new ConnectorRequest
        {
            Id = 99,
            MaxCurrentAmps = 20,
            ChargingStationId = station.Id
        };

        var response = await _client.PutAsJsonAsync(
            $"/stations/{station.Id}/connectors/99", update);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PUT_connectors_WithMissingStation_ShouldReturnNotFound()
    {
        var missingStationId = Guid.NewGuid();

        var response = await _client.PutAsJsonAsync(
            $"/stations/{missingStationId}/connectors/1",
            new ConnectorRequest { Id = 1, MaxCurrentAmps = 10, ChargingStationId = missingStationId });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DELETE_connectors_WithMissingConnector_ShouldReturnNotFound()
    {
        var group = await Helper.CreateGroup(_client);
        var station = await Helper.CreateStation(_client, group.Id);

        var response = await _client.DeleteAsync($"/stations/{station.Id}/connectors/99");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DELETE_connectors_WithMissingStation_ShouldReturnNotFound()
    {
        var missingStationId = Guid.NewGuid();

        var response = await _client.DeleteAsync($"/stations/{missingStationId}/connectors/1");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
