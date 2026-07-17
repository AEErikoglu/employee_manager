using backend.Dtos;

namespace backend.Services;

/// <summary>
/// Provides CRUD operations for work logs.
/// </summary>
public interface IWorkLogService
{
    /// <summary>
    /// Returns all work logs.
    /// </summary>
    Task<IReadOnlyList<WorkLogDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a single work log by identifier.
    /// </summary>
    Task<WorkLogDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new work log.
    /// </summary>
    Task<WorkLogDto> CreateAsync(CreateWorkLogDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing work log.
    /// </summary>
    Task<WorkLogDto?> UpdateAsync(Guid id, UpdateWorkLogDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an existing work log.
    /// </summary>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
