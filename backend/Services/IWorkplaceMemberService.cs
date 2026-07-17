using backend.Dtos;

namespace backend.Services;

/// <summary>
/// Provides CRUD operations for workplace memberships.
/// </summary>
public interface IWorkplaceMemberService
{
    /// <summary>
    /// Returns all workplace memberships.
    /// </summary>
    Task<IReadOnlyList<WorkplaceMemberDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a single workplace membership by identifier.
    /// </summary>
    Task<WorkplaceMemberDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new workplace membership.
    /// </summary>
    Task<WorkplaceMemberDto> CreateAsync(CreateWorkplaceMemberDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing workplace membership.
    /// </summary>
    Task<WorkplaceMemberDto?> UpdateAsync(Guid id, UpdateWorkplaceMemberDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an existing workplace membership.
    /// </summary>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
