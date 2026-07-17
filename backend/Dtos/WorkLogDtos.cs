using backend.Models;

namespace backend.Dtos;

/// <summary>
/// Represents a work log returned by the API.
/// </summary>
public class WorkLogDto
{
    /// <summary>
    /// Gets or sets the work log identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the related shift identifier.
    /// </summary>
    public Guid ShiftId { get; set; }

    /// <summary>
    /// Gets or sets the actual worked start time.
    /// </summary>
    public TimeOnly? ActualStartTime { get; set; }

    /// <summary>
    /// Gets or sets the actual worked end time.
    /// </summary>
    public TimeOnly? ActualEndTime { get; set; }

    /// <summary>
    /// Gets or sets the work log status.
    /// </summary>
    public WorkLogStatus Status { get; set; }

    /// <summary>
    /// Gets or sets an optional note.
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
}

/// <summary>
/// Represents the request payload for creating a work log.
/// </summary>
public class CreateWorkLogDto
{
    /// <summary>
    /// Gets or sets the related shift identifier.
    /// </summary>
    public Guid ShiftId { get; set; }

    /// <summary>
    /// Gets or sets the actual worked start time.
    /// </summary>
    public TimeOnly? ActualStartTime { get; set; }

    /// <summary>
    /// Gets or sets the actual worked end time.
    /// </summary>
    public TimeOnly? ActualEndTime { get; set; }

    /// <summary>
    /// Gets or sets the work log status.
    /// </summary>
    public WorkLogStatus Status { get; set; } = WorkLogStatus.Completed;

    /// <summary>
    /// Gets or sets an optional note.
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Gets or sets the workplace member who recorded the work log.
    /// </summary>
    public Guid RecordedByMemberId { get; set; }

    /// <summary>
    /// Gets or sets when the work log was recorded.
    /// </summary>
    public DateTime? RecordedAt { get; set; }
}

/// <summary>
/// Represents the request payload for updating a work log.
/// </summary>
public class UpdateWorkLogDto
{
    /// <summary>
    /// Gets or sets the related shift identifier.
    /// </summary>
    public Guid ShiftId { get; set; }

    /// <summary>
    /// Gets or sets the actual worked start time.
    /// </summary>
    public TimeOnly? ActualStartTime { get; set; }

    /// <summary>
    /// Gets or sets the actual worked end time.
    /// </summary>
    public TimeOnly? ActualEndTime { get; set; }

    /// <summary>
    /// Gets or sets the work log status.
    /// </summary>
    public WorkLogStatus Status { get; set; } = WorkLogStatus.Completed;

    /// <summary>
    /// Gets or sets an optional note.
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Gets or sets the workplace member who recorded the work log.
    /// </summary>
    public Guid RecordedByMemberId { get; set; }

    /// <summary>
    /// Gets or sets when the work log was recorded.
    /// </summary>
    public DateTime? RecordedAt { get; set; }
}
