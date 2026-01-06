using EVCharging.Dtos;
using System.Net.Http.Json;

namespace EVChargingTests.Controllers;

/// <summary>
/// Helper methods shared across integration tests for creating common test data
/// through real HTTP calls. Keeps tests clean and reduces duplication.
/// </summary>
public static class Helper
{
    public static async Task<GroupResponse> CreateGroup(HttpClient client,
        string name = "G1", int capacity = 100)
    {
        var response = await client.PostAsJsonAsync("/groups",
            new GroupRequest { Name = name, CapacityAmps = capacity });

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<GroupResponse>()
               ?? throw new Exception("No JSON returned for group");
    }

    public static async Task<ChargingStationResponse> CreateStation(HttpClient client,
        Guid groupId, string name = "S1", int amps = 10)
    {
        var request = new CreateChargingStationRequest
        {
            Name = name,
            GroupId = groupId,
            Connectors = new()
            {
                new CreateConnectorRequest { Id = 1, MaxCurrentAmps = amps }
            }
        };

        var response = await client.PostAsJsonAsync("/stations", request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ChargingStationResponse>()
               ?? throw new Exception("No JSON returned for station");
    }
}
