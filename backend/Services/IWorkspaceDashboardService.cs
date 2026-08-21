using System.Security.Claims;
using backend.Dtos;

namespace backend.Services;

/// <summary>
/// Provides authenticated users with their workspace dashboard data.
/// </summary>
public interface IWorkspaceDashboardService
{
    /// <summary>
    /// Returns the workspaces available to the authenticated user.
    /// </summary>
    Task<IReadOnlyList<WorkspaceDashboardDto>> GetWorkspacesAsync(
        ClaimsPrincipal principal,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a workspace for the authenticated user and assigns the manager role.
    /// </summary>
    Task<WorkspaceDashboardDto> CreateWorkspaceAsync(
        ClaimsPrincipal principal,
        CreateDashboardWorkspaceDto request,
        CancellationToken cancellationToken = default);
}
