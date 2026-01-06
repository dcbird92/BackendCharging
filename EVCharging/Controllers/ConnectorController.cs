using EVCharging.Dtos;
using EVCharging.Services;
using Microsoft.AspNetCore.Mvc;

namespace EVCharging.Controllers;

/// <summary>
/// REST API controller responsible for managing connectors within a charging station.
/// Provides endpoints to create, update, and delete connectors while delegating all
/// domain logic and validation to the ConnectorService. Ensures DTO-based responses
/// and maintains a consistent API surface.
/// </summary>
[ApiController]
[Route("stations/{stationId}/connectors")]
public class ConnectorController(ConnectorService connectorService) : ControllerBase
{
    private readonly ConnectorService _connectorService = connectorService;

    // GET all action
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ConnectorResponse>>> GetAll(Guid stationId)
    {
        var connectors = await _connectorService.GetAllAsync();
        return Ok(connectors.Where(c => c.ChargingStationId == stationId));
    }

    // GET by Id action
    [HttpGet("{connectorId:int}")]
    public async Task<ActionResult<ConnectorResponse>> Get(Guid stationId, int connectorId)
    {
        var connector = await _connectorService.GetAsync(stationId, connectorId);
        return connector is not null ? Ok(connector) : NotFound();
    }

    // POST action
    [HttpPost]
    public async Task<ActionResult<ConnectorResponse>> Create(Guid stationId, ConnectorRequest request)
    {
        try
        {
            request.ChargingStationId = stationId;

            var created = await _connectorService.CreateAsync(request);

            return CreatedAtAction(nameof(Get),
                new { stationId = stationId, connectorId = created.Id },
                created);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // PUT action
    [HttpPut("{connectorId:int}")]
    public async Task<ActionResult<ConnectorResponse>> Update(Guid stationId, int connectorId, ConnectorRequest request)
    {
        try
        {
            request.ChargingStationId = stationId;

            var updated = await _connectorService.UpdateAsync(stationId, connectorId, request);
            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // DELETE action
    [HttpDelete("{connectorId:int}")]
    public async Task<IActionResult> Delete(Guid stationId, int connectorId)
    {
        try
        {
            var deleted = await _connectorService.DeleteAsync(stationId, connectorId);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch(InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }

    }
}
