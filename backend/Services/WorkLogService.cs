using backend.Dtos;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

/// <summary>
/// Implements CRUD operations for work logs.
/// </summary>
public class WorkLogService(EmployeeManagerDbContext dbContext) : IWorkLogService
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<WorkLogDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.WorkLogs
            .AsNoTracking()
            .OrderBy(x => x.RecordedAt)
            .Select(MapToDto())
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<WorkLogDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.WorkLogs
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(MapToDto())
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<WorkLogDto> CreateAsync(CreateWorkLogDto request, CancellationToken cancellationToken = default)
    {
        await ValidateWorkLogAsync(
            request.ShiftId,
            request.RecordedByMemberId,
            request.ActualStartTime,
            request.ActualEndTime,
            request.Status,
            null,
            cancellationToken);

        var workLog = new WorkLog
        {
            Id = Guid.NewGuid(),
            ShiftId = request.ShiftId,
            ActualStartTime = request.ActualStartTime,
            ActualEndTime = request.ActualEndTime,
            Status = request.Status,
            Note = NormalizeOptional(request.Note),
            RecordedByMemberId = request.RecordedByMemberId
        };

        if (request.RecordedAt.HasValue)
        {
            workLog.RecordedAt = request.RecordedAt.Value;
        }

        dbContext.WorkLogs.Add(workLog);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(workLog);
    }

    /// <inheritdoc />
    public async Task<WorkLogDto?> UpdateAsync(Guid id, UpdateWorkLogDto request, CancellationToken cancellationToken = default)
    {
        var workLog = await dbContext.WorkLogs.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (workLog is null)
        {
            return null;
        }

        await ValidateWorkLogAsync(
            request.ShiftId,
            request.RecordedByMemberId,
            request.ActualStartTime,
            request.ActualEndTime,
            request.Status,
            id,
            cancellationToken);

        workLog.ShiftId = request.ShiftId;
        workLog.ActualStartTime = request.ActualStartTime;
        workLog.ActualEndTime = request.ActualEndTime;
        workLog.Status = request.Status;
        workLog.Note = NormalizeOptional(request.Note);
        workLog.RecordedByMemberId = request.RecordedByMemberId;

        if (request.RecordedAt.HasValue)
        {
            workLog.RecordedAt = request.RecordedAt.Value;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(workLog);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var workLog = await dbContext.WorkLogs.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (workLog is null)
        {
            return false;
        }

        dbContext.WorkLogs.Remove(workLog);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <summary>
    /// Validates a work log payload before create or update.
    /// </summary>
    private async Task ValidateWorkLogAsync(
        Guid shiftId,
        Guid recordedByMemberId,
        TimeOnly? actualStartTime,
        TimeOnly? actualEndTime,
        WorkLogStatus status,
        Guid? currentWorkLogId,
        CancellationToken cancellationToken)
    {
        if (actualStartTime.HasValue && actualEndTime.HasValue && actualStartTime.Value >= actualEndTime.Value)
        {
            throw new ArgumentException("Actual start time must be earlier than actual end time.", nameof(actualStartTime));
        }

        if (!Enum.IsDefined(status))
        {
            throw new ArgumentException("The provided work log status is invalid.", nameof(status));
        }

        var shift = await dbContext.Shifts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == shiftId, cancellationToken);

        if (shift is null)
        {
            throw new InvalidOperationException($"Shift '{shiftId}' was not found.");
        }

        var shiftAlreadyLogged = await dbContext.WorkLogs
            .AsNoTracking()
            .AnyAsync(x => x.ShiftId == shiftId && (!currentWorkLogId.HasValue || x.Id != currentWorkLogId.Value), cancellationToken);

        if (shiftAlreadyLogged)
        {
            throw new InvalidOperationException("A work log already exists for the specified shift.");
        }

        var recordedByMember = await dbContext.WorkplaceMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == recordedByMemberId, cancellationToken);

        if (recordedByMember is null)
        {
            throw new InvalidOperationException($"Workplace member '{recordedByMemberId}' was not found.");
        }

        if (recordedByMember.WorkplaceId != shift.WorkplaceId)
        {
            throw new InvalidOperationException("The recording workplace member must belong to the same workplace as the shift.");
        }
    }

    /// <summary>
    /// Normalizes optional note text.
    /// </summary>
    private static string? NormalizeOptional(string? value)
    {
        var normalizedValue = value?.Trim();
        return string.IsNullOrWhiteSpace(normalizedValue) ? null : normalizedValue;
    }

    /// <summary>
    /// Builds a projection from the entity to the API response DTO.
    /// </summary>
    private static System.Linq.Expressions.Expression<Func<WorkLog, WorkLogDto>> MapToDto()
    {
        return workLog => new WorkLogDto
        {
            Id = workLog.Id,
            ShiftId = workLog.ShiftId,
            ActualStartTime = workLog.ActualStartTime,
            ActualEndTime = workLog.ActualEndTime,
            Status = workLog.Status,
            Note = workLog.Note,
            RecordedByMemberId = workLog.RecordedByMemberId,
            RecordedAt = workLog.RecordedAt
        };
    }

    /// <summary>
    /// Maps an already loaded entity to the API response DTO.
    /// </summary>
    private static WorkLogDto MapToDto(WorkLog workLog)
    {
        return new WorkLogDto
        {
            Id = workLog.Id,
            ShiftId = workLog.ShiftId,
            ActualStartTime = workLog.ActualStartTime,
            ActualEndTime = workLog.ActualEndTime,
            Status = workLog.Status,
            Note = workLog.Note,
            RecordedByMemberId = workLog.RecordedByMemberId,
            RecordedAt = workLog.RecordedAt
        };
    }
}
