namespace EVCharging.Models;

/// <summary>
/// Model representing a group.
/// A group defines the maximum available amperage, and contains one or more
/// charging stations. All domain rules related to total power capacity
/// are evaluated against this entity.
/// </summary>
public class Group
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public required int CapacityAmps { get; set; }
    public ICollection<ChargingStation> ChargingStations { get; set; } = new List<ChargingStation>();
}
