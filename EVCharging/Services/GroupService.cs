using EVCharging.Models;
using EVCharging.Data;
using EVCharging.Dtos;
using Microsoft.EntityFrameworkCore;

namespace EVCharging.Services;

/// <summary>
/// Application service responsible for all group-level operations.
/// Encapsulates creation, updates, deletion, and validation logic
/// before committing changes to the database.
/// </summary>
public class GroupService(EvChargingDbContext db, EvValidator validator)
{
    private readonly EvChargingDbContext _db = db;
    private readonly EvValidator _validator = validator;

    public async Task<IEnumerable<GroupResponse>> GetAllAsync()
    {
        var groups = await _db.Groups.ToListAsync();

        return groups.Select(MapToResponse);
    }

    public async Task<GroupResponse?> GetAsync(Guid id)
    {
        var group = await _db.Groups.FirstOrDefaultAsync(g => g.Id == id);

        if (group is null)
            return null;

        return MapToResponse(group);
    }

    public async Task<GroupResponse> CreateAsync(GroupRequest request)
    {
        var group = new Group
        {
            Name = request.Name,
            CapacityAmps = request.CapacityAmps
        };

        _validator.ValidateGroup(group);

        _db.Groups.Add(group);
        await _db.SaveChangesAsync();

        return MapToResponse(group);
    }

    public async Task<GroupResponse> UpdateAsync(Guid id, GroupRequest request)
    {

        var existingGroup = await _db.Groups
                .Include(g => g.ChargingStations)
                .ThenInclude(s => s.Connectors)
                .FirstOrDefaultAsync(g => g.Id == id);

        if (existingGroup is null)
            throw new KeyNotFoundException("Group was not found");

        existingGroup.Name = request.Name;
        existingGroup.CapacityAmps = request.CapacityAmps;

        _validator.ValidateGroup(existingGroup);
        _validator.ValidateGroupCapacity(existingGroup);

        await _db.SaveChangesAsync();

        return MapToResponse(existingGroup);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existingGroup = await _db.Groups.FindAsync(id);

        if (existingGroup is null)
            return false;

        _db.Groups.Remove(existingGroup);
        await _db.SaveChangesAsync();
        return true;
    }

    private static GroupResponse MapToResponse(Group group)
    {
        return new GroupResponse
        {
            Id = group.Id,
            Name = group.Name,
            CapacityAmps = group.CapacityAmps
        };
    }
}
