namespace backend.Models;

public class AvailabilitySlot
{
    public Guid Id { get; set; }

    public Guid WorkplaceMemberId { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public DateTime CreatedAt { get; set; }

    public WorkplaceMember WorkplaceMember { get; set; } = null!;
}
