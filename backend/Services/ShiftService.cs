using backend.Dtos;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

/// <summary>
/// Implements CRUD operations for shifts.
/// </summary>
public class ShiftService(EmployeeManagerDbContext dbContext) : IShiftService
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<ShiftDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Shifts
            .AsNoTracking()
            .OrderBy(x => x.Date)
            .ThenBy(x => x.StartTime)
            .Select(MapToDto())
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ShiftDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Shifts
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(MapToDto())
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ShiftDto> CreateAsync(CreateShiftDto request, CancellationToken cancellationToken = default)
    {
        await ValidateShiftAsync(
            request.WorkplaceId,
            request.WorkplaceMemberId,
            request.AssignedByMemberId,
            request.StartTime,
            request.EndTime,
            cancellationToken);

        var shift = new Shift
        {
            Id = Guid.NewGuid(),
            WorkplaceId = request.WorkplaceId,
            WorkplaceMemberId = request.WorkplaceMemberId,
            Date = request.Date,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            AssignedByMemberId = request.AssignedByMemberId
        };

        dbContext.Shifts.Add(shift);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(shift);
    }

    /// <inheritdoc />
    public async Task<ShiftDto?> UpdateAsync(Guid id, UpdateShiftDto request, CancellationToken cancellationToken = default)
    {
        var shift = await dbContext.Shifts.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (shift is null)
        {
            return null;
        }

        await ValidateShiftAsync(
            request.WorkplaceId,
            request.WorkplaceMemberId,
            request.AssignedByMemberId,
            request.StartTime,
            request.EndTime,
            cancellationToken);

        shift.WorkplaceId = request.WorkplaceId;
        shift.WorkplaceMemberId = request.WorkplaceMemberId;
        shift.Date = request.Date;
        shift.StartTime = request.StartTime;
        shift.EndTime = request.EndTime;
        shift.AssignedByMemberId = request.AssignedByMemberId;

        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(shift);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var shift = await dbContext.Shifts.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (shift is null)
        {
            return false;
        }

        dbContext.Shifts.Remove(shift);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <summary>
    /// Validates a shift payload before create or update.
    /// </summary>
    private async Task ValidateShiftAsync(
        Guid workplaceId,
        Guid workplaceMemberId,
        Guid assignedByMemberId,
        TimeOnly startTime,
        TimeOnly endTime,
        CancellationToken cancellationToken)
    {
        if (startTime >= endTime)
        {
            throw new ArgumentException("Start time must be earlier than end time.", nameof(startTime));
        }

        var workplaceExists = await dbContext.Workplaces
            .AsNoTracking()
            .AnyAsync(x => x.Id == workplaceId, cancellationToken);

        if (!workplaceExists)
        {
            throw new InvalidOperationException($"Workplace '{workplaceId}' was not found.");
        }

        var workplaceMember = await dbContext.WorkplaceMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == workplaceMemberId, cancellationToken);

        if (workplaceMember is null)
        {
            throw new InvalidOperationException($"Workplace member '{workplaceMemberId}' was not found.");
        }

        if (workplaceMember.WorkplaceId != workplaceId)
        {
            throw new InvalidOperationException("The assigned workplace member must belong to the target workplace.");
        }

        var assignedByMember = await dbContext.WorkplaceMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == assignedByMemberId, cancellationToken);

        if (assignedByMember is null)
        {
            throw new InvalidOperationException($"Workplace member '{assignedByMemberId}' was not found.");
        }

        if (assignedByMember.WorkplaceId != workplaceId)
        {
            throw new InvalidOperationException("The assigning workplace member must belong to the target workplace.");
        }
    }

    /// <summary>
    /// Builds a projection from the entity to the API response DTO.
    /// </summary>
    private static System.Linq.Expressions.Expression<Func<Shift, ShiftDto>> MapToDto()
    {
        return shift => new ShiftDto
        {
            Id = shift.Id,
            WorkplaceId = shift.WorkplaceId,
            WorkplaceMemberId = shift.WorkplaceMemberId,
            Date = shift.Date,
            StartTime = shift.StartTime,
            EndTime = shift.EndTime,
            AssignedByMemberId = shift.AssignedByMemberId,
            CreatedAt = shift.CreatedAt
        };
    }

    /// <summary>
    /// Maps an already loaded entity to the API response DTO.
    /// </summary>
    private static ShiftDto MapToDto(Shift shift)
    {
        return new ShiftDto
        {
            Id = shift.Id,
            WorkplaceId = shift.WorkplaceId,
            WorkplaceMemberId = shift.WorkplaceMemberId,
            Date = shift.Date,
            StartTime = shift.StartTime,
            EndTime = shift.EndTime,
            AssignedByMemberId = shift.AssignedByMemberId,
            CreatedAt = shift.CreatedAt
        };
    }
}
