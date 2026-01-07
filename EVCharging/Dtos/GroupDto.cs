using System.ComponentModel.DataAnnotations;

namespace EVCharging.Dtos;

/// <summary>
/// Input DTO used when creating or updating a group.
/// Represents only the fields that clients are allowed to supply.
/// </summary>
public class GroupRequest
{
    [Required]
    [MinLength(1, ErrorMessage = "Group name is required")]
    public required string Name { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "CapacityAmps must be greater than 0")]
    public required int CapacityAmps { get; set; }
}

/// <summary>
/// Output DTO returned to clients after group operations.
/// Provides a clean, client-facing representation of the Group entity.
/// </summary>
public class GroupResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required int CapacityAmps { get; set; }
}