namespace backend.Dtos;

/// <summary>
/// Represents an availability slot returned by the API.
/// </summary>
public class AvailabilitySlotDto
{
    /// <summary>
    /// Gets or sets the availability slot identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the workplace member identifier.
    /// </summary>
    public Guid WorkplaceMemberId { get; set; }

    /// <summary>
    /// Gets or sets the calendar date of the availability slot.
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Gets or sets the start time of the availability slot.
    /// </summary>
    public TimeOnly StartTime { get; set; }

    /// <summary>
    /// Gets or sets the end time of the availability slot.
    /// </summary>
    public TimeOnly EndTime { get; set; }

    /// <summary>
    /// Gets or sets when the availability slot was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Represents the request payload for creating an availability slot.
/// </summary>
public class CreateAvailabilitySlotDto
{
    /// <summary>
    /// Gets or sets the workplace member identifier.
    /// </summary>
    public Guid WorkplaceMemberId { get; set; }

    /// <summary>
    /// Gets or sets the calendar date of the availability slot.
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Gets or sets the start time of the availability slot.
    /// </summary>
    public TimeOnly StartTime { get; set; }

    /// <summary>
    /// Gets or sets the end time of the availability slot.
    /// </summary>
    public TimeOnly EndTime { get; set; }
}

/// <summary>
/// Represents the request payload for updating an availability slot.
/// </summary>
public class UpdateAvailabilitySlotDto
{
    /// <summary>
    /// Gets or sets the workplace member identifier.
    /// </summary>
    public Guid WorkplaceMemberId { get; set; }

    /// <summary>
    /// Gets or sets the calendar date of the availability slot.
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Gets or sets the start time of the availability slot.
    /// </summary>
    public TimeOnly StartTime { get; set; }

    /// <summary>
    /// Gets or sets the end time of the availability slot.
    /// </summary>
    public TimeOnly EndTime { get; set; }
}
