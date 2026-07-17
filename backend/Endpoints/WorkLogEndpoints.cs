using backend.Dtos;
using backend.Services;

namespace backend.Endpoints;

/// <summary>
/// Registers work-log-related minimal API endpoints.
/// </summary>
public static class WorkLogEndpoints
{
    /// <summary>
    /// Maps CRUD endpoints for work logs.
    /// </summary>
    public static IEndpointRouteBuilder MapWorkLogEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/work-logs")
            .WithTags("WorkLogs");

        group.MapGet("/", GetAllAsync)
            .WithName("GetAllWorkLogs");

        group.MapGet("/{id:guid}", GetByIdAsync)
            .WithName("GetWorkLogById");

        group.MapPost("/", CreateAsync)
            .WithName("CreateWorkLog");

        group.MapPut("/{id:guid}", UpdateAsync)
            .WithName("UpdateWorkLog");

        group.MapDelete("/{id:guid}", DeleteAsync)
            .WithName("DeleteWorkLog");

        return endpoints;
    }

    /// <summary>
    /// Returns all work logs.
    /// </summary>
    private static async Task<IResult> GetAllAsync(IWorkLogService workLogService, CancellationToken cancellationToken)
    {
        var workLogs = await workLogService.GetAllAsync(cancellationToken);
        return Results.Ok(workLogs);
    }

    /// <summary>
    /// Returns a single work log by identifier.
    /// </summary>
    private static async Task<IResult> GetByIdAsync(Guid id, IWorkLogService workLogService, CancellationToken cancellationToken)
    {
        var workLog = await workLogService.GetByIdAsync(id, cancellationToken);
        return workLog is null ? Results.NotFound() : Results.Ok(workLog);
    }

    /// <summary>
    /// Creates a new work log.
    /// </summary>
    private static async Task<IResult> CreateAsync(
        CreateWorkLogDto request,
        IWorkLogService workLogService,
        CancellationToken cancellationToken)
    {
        try
        {
            var workLog = await workLogService.CreateAsync(request, cancellationToken);
            return Results.Created($"/work-logs/{workLog.Id}", workLog);
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
    /// Updates an existing work log.
    /// </summary>
    private static async Task<IResult> UpdateAsync(
        Guid id,
        UpdateWorkLogDto request,
        IWorkLogService workLogService,
        CancellationToken cancellationToken)
    {
        try
        {
            var workLog = await workLogService.UpdateAsync(id, request, cancellationToken);
            return workLog is null ? Results.NotFound() : Results.Ok(workLog);
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
    /// Deletes an existing work log.
    /// </summary>
    private static async Task<IResult> DeleteAsync(Guid id, IWorkLogService workLogService, CancellationToken cancellationToken)
    {
        var deleted = await workLogService.DeleteAsync(id, cancellationToken);
        return deleted ? Results.NoContent() : Results.NotFound();
    }

    /// <summary>
    /// Builds a consistent validation response payload.
    /// </summary>
    private static IResult CreateValidationProblem(string errorMessage)
    {
        return EndpointValidationResults.Create("workLog", errorMessage);
    }
}
