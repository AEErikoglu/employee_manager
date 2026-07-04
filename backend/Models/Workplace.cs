namespace backend.Models;

public class Workplace
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public Guid CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public AppUser CreatedByUser { get; set; } = null!;

    public ICollection<WorkplaceMember> WorkplaceMembers { get; set; } = [];

    public ICollection<Invitation> Invitations { get; set; } = [];

    public ICollection<Shift> Shifts { get; set; } = [];
}
