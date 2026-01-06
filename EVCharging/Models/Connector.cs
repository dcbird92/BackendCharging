namespace EVCharging.Models;

/// <summary>
/// Model representing an individual connector on a charging station.
/// Connectors have a unique ID within a station (1–5) and a maximum amperage.
/// These values participate in domain-level capacity validation.
/// </summary>

public class Connector 
{
    public required int Id { get; set; }
    public required int MaxCurrentAmps { get; set; }
    public Guid ChargingStationId { get; set; }
    public ChargingStation? ChargingStation { get; set; }
}