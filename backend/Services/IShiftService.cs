using backend.Dtos;

namespace backend.Services;

/// <summary>
/// Provides CRUD operations for shifts.
/// </summary>
public interface IShiftService
{
    /// <summary>
    /// Returns all shifts.
    /// </summary>
    Task<IReadOnlyList<ShiftDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a single shift by identifier.
    /// </summary>
    Task<ShiftDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new shift.
    /// </summary>
    Task<ShiftDto> CreateAsync(CreateShiftDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing shift.
    /// </summary>
    Task<ShiftDto?> UpdateAsync(Guid id, UpdateShiftDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an existing shift.
    /// </summary>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
