using backend.Dtos;
using backend.Services;

namespace backend.Endpoints;

/// <summary>
/// Registers dashboard endpoints scoped to the authenticated user.
/// </summary>
public static class DashboardEndpoints
{
    /// <summary>
    /// Maps the workspace dashboard endpoints.
    /// </summary>
    public static IEndpointRouteBuilder MapDashboardEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/dashboard")
            .WithTags("Dashboard")
            .RequireAuthorization();

        group.MapGet("/workspaces", GetWorkspacesAsync)
            .WithName("GetDashboardWorkspaces");

        group.MapPost("/workspaces", CreateWorkspaceAsync)
            .WithName("CreateDashboardWorkspace");

        return endpoints;
    }

    /// <summary>
    /// Returns the authenticated user's workspaces.
    /// </summary>
    private static async Task<IResult> GetWorkspacesAsync(
        HttpContext context,
        IWorkspaceDashboardService dashboardService,
        CancellationToken cancellationToken)
    {
        try
        {
            var workspaces = await dashboardService.GetWorkspacesAsync(context.User, cancellationToken);
            return Results.Ok(workspaces);
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Unauthorized();
        }
    }

    /// <summary>
    /// Creates a workspace for the authenticated user.
    /// </summary>
    private static async Task<IResult> CreateWorkspaceAsync(
        HttpContext context,
        CreateDashboardWorkspaceDto request,
        IWorkspaceDashboardService dashboardService,
        CancellationToken cancellationToken)
    {
        try
        {
            var workspace = await dashboardService.CreateWorkspaceAsync(context.User, request, cancellationToken);
            return Results.Created($"/dashboard/workspaces/{workspace.Id}", workspace);
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Unauthorized();
        }
        catch (ArgumentException exception)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["workspace"] = [exception.Message],
            });
        }
    }
}
