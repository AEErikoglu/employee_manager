using backend.Dtos;

namespace backend.Services;

/// <summary>
/// Provides CRUD operations for invitations.
/// </summary>
public interface IInvitationService
{
    /// <summary>
    /// Returns all invitations.
    /// </summary>
    Task<IReadOnlyList<InvitationDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a single invitation by identifier.
    /// </summary>
    Task<InvitationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new invitation.
    /// </summary>
    Task<InvitationDto> CreateAsync(CreateInvitationDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing invitation.
    /// </summary>
    Task<InvitationDto?> UpdateAsync(Guid id, UpdateInvitationDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an existing invitation.
    /// </summary>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
