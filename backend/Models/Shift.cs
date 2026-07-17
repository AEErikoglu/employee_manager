namespace backend.Models;

public class Shift
{
    public Guid Id { get; set; }

    public Guid WorkplaceId { get; set; }

    public Guid WorkplaceMemberId { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public Guid AssignedByMemberId { get; set; }

    public DateTime CreatedAt { get; set; }

    public Workplace Workplace { get; set; } = null!;

    public WorkplaceMember WorkplaceMember { get; set; } = null!;

    public WorkplaceMember AssignedByMember { get; set; } = null!;

    public WorkLog? WorkLog { get; set; }
}
