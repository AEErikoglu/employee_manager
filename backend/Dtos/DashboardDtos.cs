namespace backend.Dtos;

/// <summary>
/// Represents a workspace available to the authenticated user.
/// </summary>
public sealed class WorkspaceDashboardDto
{
    /// <summary>
    /// Gets or sets the workspace identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the workspace name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of members in the workspace.
    /// </summary>
    public int EmployeeCount { get; set; }

    /// <summary>
    /// Gets or sets the authenticated user's role in the workspace.
    /// </summary>
    public string Role { get; set; } = string.Empty;
}

/// <summary>
/// Represents the request payload for creating a workspace from the dashboard.
/// </summary>
public sealed class CreateDashboardWorkspaceDto
{
    /// <summary>
    /// Gets or sets the workspace name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
