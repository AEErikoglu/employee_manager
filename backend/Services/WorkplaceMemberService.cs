using backend.Dtos;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

/// <summary>
/// Implements CRUD operations for workplace memberships.
/// </summary>
public class WorkplaceMemberService(EmployeeManagerDbContext dbContext) : IWorkplaceMemberService
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<WorkplaceMemberDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.WorkplaceMembers
            .AsNoTracking()
            .OrderBy(x => x.JoinedAt)
            .Select(MapToDto())
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<WorkplaceMemberDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.WorkplaceMembers
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(MapToDto())
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<WorkplaceMemberDto> CreateAsync(CreateWorkplaceMemberDto request, CancellationToken cancellationToken = default)
    {
        ValidateRole(request.Role);

        await EnsureWorkplaceExistsAsync(request.WorkplaceId, cancellationToken);
        await EnsureUserExistsAsync(request.UserId, cancellationToken);

        var duplicateExists = await dbContext.WorkplaceMembers
            .AsNoTracking()
            .AnyAsync(x => x.WorkplaceId == request.WorkplaceId && x.UserId == request.UserId, cancellationToken);

        if (duplicateExists)
        {
            throw new InvalidOperationException("The user is already a member of this workplace.");
        }

        var workplaceMember = new WorkplaceMember
        {
            Id = Guid.NewGuid(),
            WorkplaceId = request.WorkplaceId,
            UserId = request.UserId,
            Role = request.Role
        };

        dbContext.WorkplaceMembers.Add(workplaceMember);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(workplaceMember);
    }

    /// <inheritdoc />
    public async Task<WorkplaceMemberDto?> UpdateAsync(Guid id, UpdateWorkplaceMemberDto request, CancellationToken cancellationToken = default)
    {
        var workplaceMember = await dbContext.WorkplaceMembers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (workplaceMember is null)
        {
            return null;
        }

        ValidateRole(request.Role);

        if (workplaceMember.Role == WorkplaceRole.Manager && request.Role != WorkplaceRole.Manager)
        {
            var anotherManagerExists = await dbContext.WorkplaceMembers
                .AsNoTracking()
                .AnyAsync(x => x.WorkplaceId == workplaceMember.WorkplaceId
                               && x.Id != workplaceMember.Id
                               && x.Role == WorkplaceRole.Manager, cancellationToken);

            if (!anotherManagerExists)
            {
                throw new InvalidOperationException("A workplace must always have at least one manager.");
            }
        }

        workplaceMember.Role = request.Role;

        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(workplaceMember);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var workplaceMember = await dbContext.WorkplaceMembers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (workplaceMember is null)
        {
            return false;
        }

        if (workplaceMember.Role == WorkplaceRole.Manager)
        {
            var anotherManagerExists = await dbContext.WorkplaceMembers
                .AsNoTracking()
                .AnyAsync(x => x.WorkplaceId == workplaceMember.WorkplaceId
                               && x.Id != workplaceMember.Id
                               && x.Role == WorkplaceRole.Manager, cancellationToken);

            if (!anotherManagerExists)
            {
                throw new InvalidOperationException("A workplace must always have at least one manager.");
            }
        }

        var hasSentInvitations = await dbContext.Invitations
            .AsNoTracking()
            .AnyAsync(x => x.InvitedByMemberId == id, cancellationToken);

        if (hasSentInvitations)
        {
            throw new InvalidOperationException("The workplace member cannot be deleted because invitations were sent from this membership.");
        }

        var hasAssignedShifts = await dbContext.Shifts
            .AsNoTracking()
            .AnyAsync(x => x.AssignedByMemberId == id, cancellationToken);

        if (hasAssignedShifts)
        {
            throw new InvalidOperationException("The workplace member cannot be deleted because shifts were assigned from this membership.");
        }

        var hasRecordedWorkLogs = await dbContext.WorkLogs
            .AsNoTracking()
            .AnyAsync(x => x.RecordedByMemberId == id, cancellationToken);

        if (hasRecordedWorkLogs)
        {
            throw new InvalidOperationException("The workplace member cannot be deleted because work logs were recorded from this membership.");
        }

        dbContext.WorkplaceMembers.Remove(workplaceMember);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            throw new InvalidOperationException("The workplace member cannot be deleted because related records still depend on it.");
        }

        return true;
    }

    /// <summary>
    /// Validates that the provided workplace role is supported.
    /// </summary>
    private static void ValidateRole(WorkplaceRole role)
    {
        if (!Enum.IsDefined(role))
        {
            throw new ArgumentException("The provided workplace role is invalid.", nameof(role));
        }
    }

    /// <summary>
    /// Ensures that the target workplace exists.
    /// </summary>
    private async Task EnsureWorkplaceExistsAsync(Guid workplaceId, CancellationToken cancellationToken)
    {
        var workplaceExists = await dbContext.Workplaces
            .AsNoTracking()
            .AnyAsync(x => x.Id == workplaceId, cancellationToken);

        if (!workplaceExists)
        {
            throw new InvalidOperationException($"Workplace '{workplaceId}' was not found.");
        }
    }

    /// <summary>
    /// Ensures that the target application user exists.
    /// </summary>
    private async Task EnsureUserExistsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var userExists = await dbContext.AppUsers
            .AsNoTracking()
            .AnyAsync(x => x.Id == userId, cancellationToken);

        if (!userExists)
        {
            throw new InvalidOperationException($"App user '{userId}' was not found.");
        }
    }

    /// <summary>
    /// Builds a projection from the entity to the API response DTO.
    /// </summary>
    private static System.Linq.Expressions.Expression<Func<WorkplaceMember, WorkplaceMemberDto>> MapToDto()
    {
        return workplaceMember => new WorkplaceMemberDto
        {
            Id = workplaceMember.Id,
            WorkplaceId = workplaceMember.WorkplaceId,
            UserId = workplaceMember.UserId,
            Role = workplaceMember.Role,
            JoinedAt = workplaceMember.JoinedAt
        };
    }

    /// <summary>
    /// Maps an already loaded entity to the API response DTO.
    /// </summary>
    private static WorkplaceMemberDto MapToDto(WorkplaceMember workplaceMember)
    {
        return new WorkplaceMemberDto
        {
            Id = workplaceMember.Id,
            WorkplaceId = workplaceMember.WorkplaceId,
            UserId = workplaceMember.UserId,
            Role = workplaceMember.Role,
            JoinedAt = workplaceMember.JoinedAt
        };
    }
}
