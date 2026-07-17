using backend.Models;

namespace backend.Dtos;

/// <summary>
/// Represents a workplace membership returned by the API.
/// </summary>
public class WorkplaceMemberDto
{
    /// <summary>
    /// Gets or sets the membership identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the workplace identifier.
    /// </summary>
    public Guid WorkplaceId { get; set; }

    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the member role inside the workplace.
    /// </summary>
    public WorkplaceRole Role { get; set; }

    /// <summary>
    /// Gets or sets when the membership was created.
    /// </summary>
    public DateTime JoinedAt { get; set; }
}

/// <summary>
/// Represents the request payload for creating a workplace membership.
/// </summary>
public class CreateWorkplaceMemberDto
{
    /// <summary>
    /// Gets or sets the workplace identifier.
    /// </summary>
    public Guid WorkplaceId { get; set; }

    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the member role inside the workplace.
    /// </summary>
    public WorkplaceRole Role { get; set; } = WorkplaceRole.Employee;
}

/// <summary>
/// Represents the request payload for updating a workplace membership.
/// </summary>
public class UpdateWorkplaceMemberDto
{
    /// <summary>
    /// Gets or sets the updated role inside the workplace.
    /// </summary>
    public WorkplaceRole Role { get; set; } = WorkplaceRole.Employee;
}
