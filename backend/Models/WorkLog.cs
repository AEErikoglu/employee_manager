namespace backend.Models;

/// <summary>
/// Stores the recorded execution details for a shift after it was worked or cancelled.
/// </summary>
public class WorkLog
{
    /// <summary>
    /// Gets or sets the unique identifier of the work log.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the related shift identifier.
    /// </summary>
    public Guid ShiftId { get; set; }

    /// <summary>
    /// Gets or sets the actual start time that was worked.
    /// </summary>
    public TimeOnly? ActualStartTime { get; set; }

    /// <summary>
    /// Gets or sets the actual end time that was worked.
    /// </summary>
    public TimeOnly? ActualEndTime { get; set; }

    /// <summary>
    /// Gets or sets the attendance result of the shift.
    /// </summary>
    public WorkLogStatus Status { get; set; }

    /// <summary>
    /// Gets or sets an optional note for the recorded work log.
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Gets or sets the workplace member who recorded the work log.
    /// </summary>
    public Guid RecordedByMemberId { get; set; }

    /// <summary>
    /// Gets or sets when the work log was recorded.
    /// </summary>
    public DateTime RecordedAt { get; set; }

    /// <summary>
    /// Gets or sets the related shift.
    /// </summary>
    public Shift Shift { get; set; } = null!;

    /// <summary>
    /// Gets or sets the workplace member who recorded the work log.
    /// </summary>
    public WorkplaceMember RecordedByMember { get; set; } = null!;
}
