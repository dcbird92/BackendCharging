using Asp.Versioning;
using EVCharging.Dtos;
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
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/groups")]
public class GroupController(GroupService groupService) : ControllerBase
{
    private readonly GroupService _groupService = groupService;

    // GET all action
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GroupResponse>>> GetAll()
    {
        var groups = await _groupService.GetAllAsync();
        return Ok(groups);
    }

    // GET by id action
    [HttpGet("{id}")]
    public async Task<ActionResult<GroupResponse>> Get(Guid id)
    {
        var group = await _groupService.GetAsync(id);
        return group is not null ? Ok(group) : NotFound();
    }

    // POST action
    [HttpPost]
    public async Task<ActionResult<GroupResponse>> Create(GroupRequest request)
    {
        var newGroup = await _groupService.CreateAsync(request);
        return CreatedAtAction(nameof(Get),
            new { version = HttpContext.GetRequestedApiVersion()?.ToString(), id = newGroup.Id },
            newGroup);
    }

    // PUT action
    [HttpPut("{id}")]
    public async Task<ActionResult<GroupResponse>> Update(Guid id, GroupRequest request)
    {
        var updated = await _groupService.UpdateAsync(id, request);
        return Ok(updated);
    }

    // DELETE action
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _groupService.DeleteAsync(id);
        return response ? NoContent() : NotFound();

    }
}