namespace backend.Dtos;

/// <summary>
/// Represents a shift returned by the API.
/// </summary>
public class ShiftDto
{
    /// <summary>
    /// Gets or sets the shift identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the workplace identifier.
    /// </summary>
    public Guid WorkplaceId { get; set; }

    /// <summary>
    /// Gets or sets the assigned workplace member identifier.
    /// </summary>
    public Guid WorkplaceMemberId { get; set; }

    /// <summary>
    /// Gets or sets the shift date.
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Gets or sets the planned shift start time.
    /// </summary>
    public TimeOnly StartTime { get; set; }

    /// <summary>
    /// Gets or sets the planned shift end time.
    /// </summary>
    public TimeOnly EndTime { get; set; }

    /// <summary>
    /// Gets or sets the member who assigned the shift.
    /// </summary>
    public Guid AssignedByMemberId { get; set; }

    /// <summary>
    /// Gets or sets when the shift was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Represents the request payload for creating a shift.
/// </summary>
public class CreateShiftDto
{
    /// <summary>
    /// Gets or sets the workplace identifier.
    /// </summary>
    public Guid WorkplaceId { get; set; }

    /// <summary>
    /// Gets or sets the assigned workplace member identifier.
    /// </summary>
    public Guid WorkplaceMemberId { get; set; }

    /// <summary>
    /// Gets or sets the shift date.
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Gets or sets the planned shift start time.
    /// </summary>
    public TimeOnly StartTime { get; set; }

    /// <summary>
    /// Gets or sets the planned shift end time.
    /// </summary>
    public TimeOnly EndTime { get; set; }

    /// <summary>
    /// Gets or sets the member who assigned the shift.
    /// </summary>
    public Guid AssignedByMemberId { get; set; }
}

/// <summary>
/// Represents the request payload for updating a shift.
/// </summary>
public class UpdateShiftDto
{
    /// <summary>
    /// Gets or sets the workplace identifier.
    /// </summary>
    public Guid WorkplaceId { get; set; }

    /// <summary>
    /// Gets or sets the assigned workplace member identifier.
    /// </summary>
    public Guid WorkplaceMemberId { get; set; }

    /// <summary>
    /// Gets or sets the shift date.
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Gets or sets the planned shift start time.
    /// </summary>
    public TimeOnly StartTime { get; set; }

    /// <summary>
    /// Gets or sets the planned shift end time.
    /// </summary>
    public TimeOnly EndTime { get; set; }

    /// <summary>
    /// Gets or sets the member who assigned the shift.
    /// </summary>
    public Guid AssignedByMemberId { get; set; }
}
