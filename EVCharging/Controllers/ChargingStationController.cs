using EVCharging.Dtos;
using EVCharging.Models;
using EVCharging.Services;
using Microsoft.AspNetCore.Mvc;

namespace EVCharging.Controllers;

/// <summary>
/// REST API controller responsible for managing charging groups.
/// Provides endpoints for creating, retrieving, updating, and deleting groups,
/// and delegates all business logic to the GroupService. Returns clean DTOs
/// to ensure a stable and consistent API contract.
/// </summary>
[ApiController]
[Route("stations")]
public class ChargingStationController(ChargingStationService stationService) : ControllerBase
{
    private readonly ChargingStationService _stationService = stationService;

    // GET all action
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ChargingStation>>> GetAll()
    {
        var stations = await _stationService.GetAllAsync();
        return Ok(stations);
    }

    // GET by Id action
    [HttpGet("{id}")]
    public async Task<ActionResult<ChargingStation>> Get(Guid id)
    {
        var station = await _stationService.GetAsync(id);
        return station is not null ? Ok(station) : NotFound();
    }

    // POST action
    [HttpPost]
    public async Task<ActionResult<ChargingStationResponse>> Create(CreateChargingStationRequest request)
    {
        try
        {
            var newStation = await _stationService.CreateAsync(request);
            return CreatedAtAction(nameof(Get), new { id = newStation.Id }, newStation);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // PUT action
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, ChargingStationRequest request)
    {
        try
        {
            var updated = await _stationService.UpdateAsync(id, request);
            return Ok(updated);
        }
        catch(InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // DELETE action
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _stationService.DeleteAsync(id);
        return response ? NoContent() : NotFound();
    }
}