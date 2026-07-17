using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Endpoints;

/// <summary>
/// Registers workplace-related minimal API endpoints.
/// </summary>
public static class WorkplaceEndpoints
{
    /// <summary>
    /// Maps CRUD endpoints for workplaces.
    /// </summary>
    public static IEndpointRouteBuilder MapWorkplaceEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/workplaces")
            .WithTags("Workplaces");

        group.MapGet("/", GetAllAsync)
            .WithName("GetAllWorkplaces");

        group.MapGet("/{id:guid}", GetByIdAsync)
            .WithName("GetWorkplaceById");

        group.MapPost("/", CreateAsync)
            .WithName("CreateWorkplace");

        group.MapPut("/{id:guid}", UpdateAsync)
            .WithName("UpdateWorkplace");

        group.MapDelete("/{id:guid}", DeleteAsync)
            .WithName("DeleteWorkplace");

        return endpoints;
    }

    /// <summary>
    /// Returns all workplaces.
    /// </summary>
    private static async Task<IResult> GetAllAsync(IWorkplaceService workplaceService, CancellationToken cancellationToken)
    {
        var workplaces = await workplaceService.GetAllAsync(cancellationToken);
        return Results.Ok(workplaces);
    }

    /// <summary>
    /// Returns a single workplace by its identifier.
    /// </summary>
    private static async Task<IResult> GetByIdAsync(Guid id, IWorkplaceService workplaceService, CancellationToken cancellationToken)
    {
        var workplace = await workplaceService.GetByIdAsync(id, cancellationToken);
        return workplace is null ? Results.NotFound() : Results.Ok(workplace);
    }

    /// <summary>
    /// Creates a new workplace.
    /// </summary>
    private static async Task<IResult> CreateAsync(
        CreateWorkplaceDto request,
        IWorkplaceService workplaceService,
        CancellationToken cancellationToken)
    {
        try
        {
            var workplace = await workplaceService.CreateAsync(request, cancellationToken);
            return Results.Created($"/workplaces/{workplace.Id}", workplace);
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
    /// Updates an existing workplace.
    /// </summary>
    private static async Task<IResult> UpdateAsync(
        Guid id,
        UpdateWorkplaceDto request,
        IWorkplaceService workplaceService,
        CancellationToken cancellationToken)
    {
        try
        {
            var workplace = await workplaceService.UpdateAsync(id, request, cancellationToken);
            return workplace is null ? Results.NotFound() : Results.Ok(workplace);
        }
        catch (ArgumentException exception)
        {
            return CreateValidationProblem(exception.Message);
        }
    }

    /// <summary>
    /// Deletes an existing workplace.
    /// </summary>
    private static async Task<IResult> DeleteAsync(Guid id, IWorkplaceService workplaceService, CancellationToken cancellationToken)
    {
        var deleted = await workplaceService.DeleteAsync(id, cancellationToken);
        return deleted ? Results.NoContent() : Results.NotFound();
    }

    /// <summary>
    /// Builds a consistent validation response payload.
    /// </summary>
    private static IResult CreateValidationProblem(string errorMessage)
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["workplace"] = [errorMessage]
        });
    }
}
