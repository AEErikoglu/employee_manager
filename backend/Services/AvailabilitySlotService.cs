using backend.Dtos;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

/// <summary>
/// Implements CRUD operations for availability slots.
/// </summary>
public class AvailabilitySlotService(EmployeeManagerDbContext dbContext) : IAvailabilitySlotService
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<AvailabilitySlotDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.AvailabilitySlots
            .AsNoTracking()
            .OrderBy(x => x.Date)
            .ThenBy(x => x.StartTime)
            .Select(MapToDto())
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<AvailabilitySlotDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.AvailabilitySlots
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(MapToDto())
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<AvailabilitySlotDto> CreateAsync(CreateAvailabilitySlotDto request, CancellationToken cancellationToken = default)
    {
        await ValidateAvailabilitySlotAsync(request.WorkplaceMemberId, request.StartTime, request.EndTime, cancellationToken);

        var availabilitySlot = new AvailabilitySlot
        {
            Id = Guid.NewGuid(),
            WorkplaceMemberId = request.WorkplaceMemberId,
            Date = request.Date,
            StartTime = request.StartTime,
            EndTime = request.EndTime
        };

        dbContext.AvailabilitySlots.Add(availabilitySlot);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(availabilitySlot);
    }

    /// <inheritdoc />
    public async Task<AvailabilitySlotDto?> UpdateAsync(Guid id, UpdateAvailabilitySlotDto request, CancellationToken cancellationToken = default)
    {
        var availabilitySlot = await dbContext.AvailabilitySlots.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (availabilitySlot is null)
        {
            return null;
        }

        await ValidateAvailabilitySlotAsync(request.WorkplaceMemberId, request.StartTime, request.EndTime, cancellationToken);

        availabilitySlot.WorkplaceMemberId = request.WorkplaceMemberId;
        availabilitySlot.Date = request.Date;
        availabilitySlot.StartTime = request.StartTime;
        availabilitySlot.EndTime = request.EndTime;

        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(availabilitySlot);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var availabilitySlot = await dbContext.AvailabilitySlots.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (availabilitySlot is null)
        {
            return false;
        }

        dbContext.AvailabilitySlots.Remove(availabilitySlot);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <summary>
    /// Validates an availability slot payload before create or update.
    /// </summary>
    private async Task ValidateAvailabilitySlotAsync(
        Guid workplaceMemberId,
        TimeOnly startTime,
        TimeOnly endTime,
        CancellationToken cancellationToken)
    {
        if (startTime >= endTime)
        {
            throw new ArgumentException("Start time must be earlier than end time.", nameof(startTime));
        }

        var workplaceMemberExists = await dbContext.WorkplaceMembers
            .AsNoTracking()
            .AnyAsync(x => x.Id == workplaceMemberId, cancellationToken);

        if (!workplaceMemberExists)
        {
            throw new InvalidOperationException($"Workplace member '{workplaceMemberId}' was not found.");
        }
    }

    /// <summary>
    /// Builds a projection from the entity to the API response DTO.
    /// </summary>
    private static System.Linq.Expressions.Expression<Func<AvailabilitySlot, AvailabilitySlotDto>> MapToDto()
    {
        return availabilitySlot => new AvailabilitySlotDto
        {
            Id = availabilitySlot.Id,
            WorkplaceMemberId = availabilitySlot.WorkplaceMemberId,
            Date = availabilitySlot.Date,
            StartTime = availabilitySlot.StartTime,
            EndTime = availabilitySlot.EndTime,
            CreatedAt = availabilitySlot.CreatedAt
        };
    }

    /// <summary>
    /// Maps an already loaded entity to the API response DTO.
    /// </summary>
    private static AvailabilitySlotDto MapToDto(AvailabilitySlot availabilitySlot)
    {
        return new AvailabilitySlotDto
        {
            Id = availabilitySlot.Id,
            WorkplaceMemberId = availabilitySlot.WorkplaceMemberId,
            Date = availabilitySlot.Date,
            StartTime = availabilitySlot.StartTime,
            EndTime = availabilitySlot.EndTime,
            CreatedAt = availabilitySlot.CreatedAt
        };
    }
}
