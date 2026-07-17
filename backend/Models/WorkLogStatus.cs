namespace backend.Models;

/// <summary>
/// Represents the final attendance outcome for a scheduled shift.
/// </summary>
public enum WorkLogStatus
{
    Completed,
    PartiallyWorked,
    Absent,
    Cancelled
}
