using backend.Dtos;

namespace backend.Services;

/// <summary>
/// Provides CRUD operations for application users.
/// </summary>
public interface IAppUserService
{
    /// <summary>
    /// Returns all application users.
    /// </summary>
    Task<IReadOnlyList<AppUserDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a single application user by identifier.
    /// </summary>
    Task<AppUserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new application user.
    /// </summary>
    Task<AppUserDto> CreateAsync(CreateAppUserDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing application user.
    /// </summary>
    Task<AppUserDto?> UpdateAsync(Guid id, UpdateAppUserDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an existing application user.
    /// </summary>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
