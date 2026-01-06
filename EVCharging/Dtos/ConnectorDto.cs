namespace EVCharging.Dtos;

/// <summary>
/// Input DTO for creating a connector.
/// Includes connector ID, amperage.
/// </summary>
public class CreateConnectorRequest
{
    public required int Id { get; set; }
    public required int MaxCurrentAmps { get; set; }
}

/// <summary>
/// Input DTO for updating a connector.
/// Includes connector ID, amperage, and the owning station relationship.
/// </summary>
public class ConnectorRequest
{
    public required int Id { get; set; }
    public required int MaxCurrentAmps { get; set; }
    public required Guid ChargingStationId { get; set; }
}

/// <summary>
/// Response DTO representing a connector in API output.
/// Ensures internal domain structures are not exposed to clients.
/// </summary>
public class ConnectorResponse 
{
    public required int Id { get; set; }
    public required int MaxCurrentAmps { get; set; }
    public required Guid ChargingStationId { get; set; }
}
