using backend.Dtos;

namespace backend.Services;

/// <summary>
/// Provides CRUD operations for workplaces.
/// </summary>
public interface IWorkplaceService
{
    /// <summary>
    /// Returns all workplaces.
    /// </summary>
    Task<IReadOnlyList<WorkplaceDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a single workplace by its identifier.
    /// </summary>
    Task<WorkplaceDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new workplace.
    /// </summary>
    Task<WorkplaceDto> CreateAsync(CreateWorkplaceDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing workplace.
    /// </summary>
    Task<WorkplaceDto?> UpdateAsync(Guid id, UpdateWorkplaceDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an existing workplace.
    /// </summary>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
