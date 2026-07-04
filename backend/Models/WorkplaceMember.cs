namespace backend.Models;

public class WorkplaceMember
{
    public Guid Id { get; set; }

    public Guid WorkplaceId { get; set; }

    public Guid UserId { get; set; }

    public WorkplaceRole Role { get; set; }

    public DateTime JoinedAt { get; set; }

    public Workplace Workplace { get; set; } = null!;

    public AppUser User { get; set; } = null!;

    public ICollection<AvailabilitySlot> AvailabilitySlots { get; set; } = [];

    public ICollection<Invitation> InvitationsSent { get; set; } = [];

    public ICollection<Invitation> AcceptedInvitations { get; set; } = [];

    public ICollection<Shift> ReceivedShifts { get; set; } = [];

    public ICollection<Shift> AssignedShifts { get; set; } = [];
}
