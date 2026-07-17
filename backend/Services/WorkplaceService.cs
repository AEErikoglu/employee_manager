using backend.Dtos;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

/// <summary>
/// Implements CRUD operations for workplaces.
/// </summary>
public class WorkplaceService(EmployeeManagerDbContext dbContext) : IWorkplaceService
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<WorkplaceDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Workplaces
            .AsNoTracking()
            .OrderBy(x => x.CreatedAt)
            .Select(MapToDto())
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<WorkplaceDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Workplaces
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(MapToDto())
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<WorkplaceDto> CreateAsync(CreateWorkplaceDto request, CancellationToken cancellationToken = default)
    {
        var normalizedName = NormalizeName(request.Name);

        var creatorExists = await dbContext.AppUsers
            .AsNoTracking()
            .AnyAsync(x => x.Id == request.CreatedByUserId, cancellationToken);

        if (!creatorExists)
        {
            throw new InvalidOperationException($"App user '{request.CreatedByUserId}' was not found.");
        }

        var workplace = new Workplace
        {
            Id = Guid.NewGuid(),
            Name = normalizedName,
            CreatedByUserId = request.CreatedByUserId
        };

        var managerMembership = new WorkplaceMember
        {
            Id = Guid.NewGuid(),
            WorkplaceId = workplace.Id,
            UserId = request.CreatedByUserId,
            Role = WorkplaceRole.Manager
        };

        dbContext.Workplaces.Add(workplace);
        dbContext.WorkplaceMembers.Add(managerMembership);

        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(workplace);
    }

    /// <inheritdoc />
    public async Task<WorkplaceDto?> UpdateAsync(Guid id, UpdateWorkplaceDto request, CancellationToken cancellationToken = default)
    {
        var workplace = await dbContext.Workplaces.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (workplace is null)
        {
            return null;
        }

        workplace.Name = NormalizeName(request.Name);

        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(workplace);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var workplace = await dbContext.Workplaces.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (workplace is null)
        {
            return false;
        }

        dbContext.Workplaces.Remove(workplace);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <summary>
    /// Validates and normalizes workplace names before they are stored.
    /// </summary>
    private static string NormalizeName(string name)
    {
        var normalizedName = name.Trim();

        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            throw new ArgumentException("Workplace name is required.", nameof(name));
        }

        if (normalizedName.Length > 200)
        {
            throw new ArgumentException("Workplace name must not exceed 200 characters.", nameof(name));
        }

        return normalizedName;
    }

    /// <summary>
    /// Builds a projection from the entity to the API response DTO.
    /// </summary>
    private static System.Linq.Expressions.Expression<Func<Workplace, WorkplaceDto>> MapToDto()
    {
        return workplace => new WorkplaceDto
        {
            Id = workplace.Id,
            Name = workplace.Name,
            CreatedByUserId = workplace.CreatedByUserId,
            CreatedAt = workplace.CreatedAt
        };
    }

    /// <summary>
    /// Maps an already loaded workplace entity to the API response DTO.
    /// </summary>
    private static WorkplaceDto MapToDto(Workplace workplace)
    {
        return new WorkplaceDto
        {
            Id = workplace.Id,
            Name = workplace.Name,
            CreatedByUserId = workplace.CreatedByUserId,
            CreatedAt = workplace.CreatedAt
        };
    }
}
