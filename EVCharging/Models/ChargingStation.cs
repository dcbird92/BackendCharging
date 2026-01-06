namespace EVCharging.Models;

/// <summary>
/// Model representing a charging station belonging to a specific group.
/// Each station must contain 1–5 connectors and contributes
/// to the group's total amperage consumption.
/// </summary>
public class ChargingStation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public required Guid GroupId { get; set; }
    public Group? Group { get; set; }
    public ICollection<Connector> Connectors { get; set; } = new List<Connector>();
}