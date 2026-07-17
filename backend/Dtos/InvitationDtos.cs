using backend.Models;

namespace backend.Dtos;

/// <summary>
/// Represents an invitation returned by the API.
/// </summary>
public class InvitationDto
{
    /// <summary>
    /// Gets or sets the invitation identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the workplace identifier.
    /// </summary>
    public Guid WorkplaceId { get; set; }

    /// <summary>
    /// Gets or sets the invited email address.
    /// </summary>
    public string InvitedEmail { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the invited workplace role.
    /// </summary>
    public WorkplaceRole InvitedRole { get; set; }

    /// <summary>
    /// Gets or sets the invitation token.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the invitation status.
    /// </summary>
    public InvitationStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the member who sent the invitation.
    /// </summary>
    public Guid InvitedByMemberId { get; set; }

    /// <summary>
    /// Gets or sets the accepted member identifier when available.
    /// </summary>
    public Guid? AcceptedByMemberId { get; set; }

    /// <summary>
    /// Gets or sets when the invitation was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets when the invitation expires.
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets when the invitation was accepted.
    /// </summary>
    public DateTime? AcceptedAt { get; set; }
}

/// <summary>
/// Represents the request payload for creating an invitation.
/// </summary>
public class CreateInvitationDto
{
    /// <summary>
    /// Gets or sets the workplace identifier.
    /// </summary>
    public Guid WorkplaceId { get; set; }

    /// <summary>
    /// Gets or sets the invited email address.
    /// </summary>
    public string InvitedEmail { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the invited workplace role.
    /// </summary>
    public WorkplaceRole InvitedRole { get; set; } = WorkplaceRole.Employee;

    /// <summary>
    /// Gets or sets the invitation token.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the invitation status.
    /// </summary>
    public InvitationStatus Status { get; set; } = InvitationStatus.Pending;

    /// <summary>
    /// Gets or sets the member who sent the invitation.
    /// </summary>
    public Guid InvitedByMemberId { get; set; }

    /// <summary>
    /// Gets or sets the accepted member identifier when available.
    /// </summary>
    public Guid? AcceptedByMemberId { get; set; }

    /// <summary>
    /// Gets or sets when the invitation expires.
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets when the invitation was accepted.
    /// </summary>
    public DateTime? AcceptedAt { get; set; }
}

/// <summary>
/// Represents the request payload for updating an invitation.
/// </summary>
public class UpdateInvitationDto
{
    /// <summary>
    /// Gets or sets the workplace identifier.
    /// </summary>
    public Guid WorkplaceId { get; set; }

    /// <summary>
    /// Gets or sets the invited email address.
    /// </summary>
    public string InvitedEmail { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the invited workplace role.
    /// </summary>
    public WorkplaceRole InvitedRole { get; set; } = WorkplaceRole.Employee;

    /// <summary>
    /// Gets or sets the invitation token.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the invitation status.
    /// </summary>
    public InvitationStatus Status { get; set; } = InvitationStatus.Pending;

    /// <summary>
    /// Gets or sets the member who sent the invitation.
    /// </summary>
    public Guid InvitedByMemberId { get; set; }

    /// <summary>
    /// Gets or sets the accepted member identifier when available.
    /// </summary>
    public Guid? AcceptedByMemberId { get; set; }

    /// <summary>
    /// Gets or sets when the invitation expires.
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets when the invitation was accepted.
    /// </summary>
    public DateTime? AcceptedAt { get; set; }
}
