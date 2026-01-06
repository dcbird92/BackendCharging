using EVCharging.Dtos;
using EVChargingTests.Utility;
using System.Net;
using System.Net.Http.Json;

namespace EVChargingTests.Controllers;

/// <summary>
/// End-to-end tests for CharginStationController using real HTTP requests.
/// Confirms routing, DTO binding, validation behavior, CRUD operations,
/// and database persistence through the API pipeline.
/// </summary>
public class ChargingStationContollerTests : IClassFixture<WebAppFactory>
{
    private readonly HttpClient _client;

    public ChargingStationContollerTests(WebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task POST_stations_ShouldCreateStation()
    {
        // Create parent group
        var group = await Helper.CreateGroup(_client, "G1");

        Assert.NotNull(group);

        var stationRequest = new CreateChargingStationRequest
        {
            Name = "S1",
            GroupId = group.Id,
            Connectors = new List<CreateConnectorRequest>
            {
                new CreateConnectorRequest
                {
                    Id = 1,
                    MaxCurrentAmps = 32
                },
                new CreateConnectorRequest
                {
                    Id = 2,
                    MaxCurrentAmps = 16
                }
            }
        };

        var response = await _client.PostAsJsonAsync($"/stations", stationRequest);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task GET_stations_id_ShouldReturnStation()
    {
        var group = await Helper.CreateGroup(_client);
        var station = await Helper.CreateStation(_client, group.Id);

        var response = await _client.GetAsync($"/stations/{station.Id}");

        var result = await response.Content.ReadFromJsonAsync<ChargingStationResponse>();

        Assert.NotNull(result);
        Assert.Equal(station.Id, result.Id);
    }

    [Fact]
    public async Task DELETE_stations_ShouldRemoveStation()
    {
        var group = await Helper.CreateGroup(_client);
        var station = await Helper.CreateStation(_client, group.Id);

        var delete = await _client.DeleteAsync($"/stations/{station.Id}");
        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);

        var get = await _client.GetAsync($"/stations/{station.Id}");
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    }

    [Fact]
    public async Task PUT_stations_WithMissingStation_ShouldReturnNotFound()
    {
        var missingId = Guid.NewGuid();

        var response = await _client.PutAsJsonAsync($"/stations/{missingId}",
            new ChargingStationRequest
            {
                Name = "Missing",
                GroupId = Guid.NewGuid(),
                Connectors = new List<CreateConnectorRequest>
                {
                    new CreateConnectorRequest{ Id = 1, MaxCurrentAmps = 10 }
                }
            });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DELETE_stations_WithMissingStation_ShouldReturnNotFound()
    {
        var missingId = Guid.NewGuid();

        var response = await _client.DeleteAsync($"/stations/{missingId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
