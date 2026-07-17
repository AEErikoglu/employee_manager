using backend.Dtos;

namespace backend.Services;

/// <summary>
/// Provides CRUD operations for availability slots.
/// </summary>
public interface IAvailabilitySlotService
{
    /// <summary>
    /// Returns all availability slots.
    /// </summary>
    Task<IReadOnlyList<AvailabilitySlotDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a single availability slot by identifier.
    /// </summary>
    Task<AvailabilitySlotDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new availability slot.
    /// </summary>
    Task<AvailabilitySlotDto> CreateAsync(CreateAvailabilitySlotDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing availability slot.
    /// </summary>
    Task<AvailabilitySlotDto?> UpdateAsync(Guid id, UpdateAvailabilitySlotDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an existing availability slot.
    /// </summary>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
