using backend.Dtos;
using backend.Services;

namespace backend.Endpoints;

/// <summary>
/// Registers workplace-member-related minimal API endpoints.
/// </summary>
public static class WorkplaceMemberEndpoints
{
    /// <summary>
    /// Maps CRUD endpoints for workplace memberships.
    /// </summary>
    public static IEndpointRouteBuilder MapWorkplaceMemberEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/workplace-members")
            .WithTags("WorkplaceMembers");

        group.MapGet("/", GetAllAsync)
            .WithName("GetAllWorkplaceMembers");

        group.MapGet("/{id:guid}", GetByIdAsync)
            .WithName("GetWorkplaceMemberById");

        group.MapPost("/", CreateAsync)
            .WithName("CreateWorkplaceMember");

        group.MapPut("/{id:guid}", UpdateAsync)
            .WithName("UpdateWorkplaceMember");

        group.MapDelete("/{id:guid}", DeleteAsync)
            .WithName("DeleteWorkplaceMember");

        return endpoints;
    }

    /// <summary>
    /// Returns all workplace memberships.
    /// </summary>
    private static async Task<IResult> GetAllAsync(IWorkplaceMemberService workplaceMemberService, CancellationToken cancellationToken)
    {
        var workplaceMembers = await workplaceMemberService.GetAllAsync(cancellationToken);
        return Results.Ok(workplaceMembers);
    }

    /// <summary>
    /// Returns a single workplace membership by identifier.
    /// </summary>
    private static async Task<IResult> GetByIdAsync(Guid id, IWorkplaceMemberService workplaceMemberService, CancellationToken cancellationToken)
    {
        var workplaceMember = await workplaceMemberService.GetByIdAsync(id, cancellationToken);
        return workplaceMember is null ? Results.NotFound() : Results.Ok(workplaceMember);
    }

    /// <summary>
    /// Creates a new workplace membership.
    /// </summary>
    private static async Task<IResult> CreateAsync(
        CreateWorkplaceMemberDto request,
        IWorkplaceMemberService workplaceMemberService,
        CancellationToken cancellationToken)
    {
        try
        {
            var workplaceMember = await workplaceMemberService.CreateAsync(request, cancellationToken);
            return Results.Created($"/workplace-members/{workplaceMember.Id}", workplaceMember);
        }
        catch (ArgumentException exception)
        {
            return CreateValidationProblem(exception.Message);
        }
        catch (InvalidOperationException exception)
        {
            return CreateValidationProblem(exception.Message);
        }
    }

    /// <summary>
    /// Updates an existing workplace membership.
    /// </summary>
    private static async Task<IResult> UpdateAsync(
        Guid id,
        UpdateWorkplaceMemberDto request,
        IWorkplaceMemberService workplaceMemberService,
        CancellationToken cancellationToken)
    {
        try
        {
            var workplaceMember = await workplaceMemberService.UpdateAsync(id, request, cancellationToken);
            return workplaceMember is null ? Results.NotFound() : Results.Ok(workplaceMember);
        }
        catch (ArgumentException exception)
        {
            return CreateValidationProblem(exception.Message);
        }
        catch (InvalidOperationException exception)
        {
            return CreateValidationProblem(exception.Message);
        }
    }

    /// <summary>
    /// Deletes an existing workplace membership.
    /// </summary>
    private static async Task<IResult> DeleteAsync(Guid id, IWorkplaceMemberService workplaceMemberService, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await workplaceMemberService.DeleteAsync(id, cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        }
        catch (InvalidOperationException exception)
        {
            return CreateValidationProblem(exception.Message);
        }
    }

    /// <summary>
    /// Builds a consistent validation response payload.
    /// </summary>
    private static IResult CreateValidationProblem(string errorMessage)
    {
        return EndpointValidationResults.Create("workplaceMember", errorMessage);
    }
}
