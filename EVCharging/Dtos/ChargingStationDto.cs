using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace EVCharging.Dtos;

/// <summary>
/// DTO used to create a new charging station.
/// Includes the station name, associated group, and its initial connectors.
/// </summary>
public class CreateChargingStationRequest
{
    [Required]
    [MinLength(1, ErrorMessage = "Station name is required")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "GroupId is required")]
    public required Guid GroupId { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "At least one connector is required")]
    [MaxLength(5, ErrorMessage = "A maximum of five connectors are allowed")]
    public required List<CreateConnectorRequest> Connectors { get; set; }
}

/// <summary>
/// General DTO used for charging station, pretty much anything other then creation.
/// Includes the station name, associated group, and its initial connectors.
/// </summary>
public class ChargingStationRequest
{
    [Required]
    [MinLength(1, ErrorMessage = "Station name is required")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "GroupId is required")]
    public required Guid GroupId { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "At least one connector is required")]
    [MaxLength(5, ErrorMessage = "A maximum of five connectors are allowed")]
    public required List<ConnectorRequest> Connectors { get; set; }
}

/// <summary>
/// DTO returned from station-related endpoints.
/// Contains the station identity, group association, and connector data.
/// </summary>
public class ChargingStationResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required Guid GroupId { get; set; }
    public required List<ConnectorResponse> Connectors { get; set; }
}