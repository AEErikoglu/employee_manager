namespace backend.Models;

public class Invitation
{
    public Guid Id { get; set; }

    public Guid WorkplaceId { get; set; }

    public string InvitedEmail { get; set; } = string.Empty;

    public WorkplaceRole InvitedRole { get; set; }

    public string Token { get; set; } = string.Empty;

    public InvitationStatus Status { get; set; }

    public Guid InvitedByMemberId { get; set; }

    public Guid? AcceptedByMemberId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime? AcceptedAt { get; set; }

    public Workplace Workplace { get; set; } = null!;

    public WorkplaceMember InvitedByMember { get; set; } = null!;

    public WorkplaceMember? AcceptedByMember { get; set; }
}
