namespace backend.Models;

public class AppUser
{
    public Guid Id { get; set; }

    public string ExternalAuthUserId { get; set; } = string.Empty;

    public string AuthProvider { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? DisplayName { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<Workplace> CreatedWorkplaces { get; set; } = [];

    public ICollection<WorkplaceMember> WorkplaceMemberships { get; set; } = [];
}
