using backend.Dtos;
using backend.Services;

namespace backend.Endpoints;

/// <summary>
/// Registers shift-related minimal API endpoints.
/// </summary>
public static class ShiftEndpoints
{
    /// <summary>
    /// Maps CRUD endpoints for shifts.
    /// </summary>
    public static IEndpointRouteBuilder MapShiftEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/shifts")
            .WithTags("Shifts");

        group.MapGet("/", GetAllAsync)
            .WithName("GetAllShifts");

        group.MapGet("/{id:guid}", GetByIdAsync)
            .WithName("GetShiftById");

        group.MapPost("/", CreateAsync)
            .WithName("CreateShift");

        group.MapPut("/{id:guid}", UpdateAsync)
            .WithName("UpdateShift");

        group.MapDelete("/{id:guid}", DeleteAsync)
            .WithName("DeleteShift");

        return endpoints;
    }

    /// <summary>
    /// Returns all shifts.
    /// </summary>
    private static async Task<IResult> GetAllAsync(IShiftService shiftService, CancellationToken cancellationToken)
    {
        var shifts = await shiftService.GetAllAsync(cancellationToken);
        return Results.Ok(shifts);
    }

    /// <summary>
    /// Returns a single shift by identifier.
    /// </summary>
    private static async Task<IResult> GetByIdAsync(Guid id, IShiftService shiftService, CancellationToken cancellationToken)
    {
        var shift = await shiftService.GetByIdAsync(id, cancellationToken);
        return shift is null ? Results.NotFound() : Results.Ok(shift);
    }

    /// <summary>
    /// Creates a new shift.
    /// </summary>
    private static async Task<IResult> CreateAsync(
        CreateShiftDto request,
        IShiftService shiftService,
        CancellationToken cancellationToken)
    {
        try
        {
            var shift = await shiftService.CreateAsync(request, cancellationToken);
            return Results.Created($"/shifts/{shift.Id}", shift);
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
    /// Updates an existing shift.
    /// </summary>
    private static async Task<IResult> UpdateAsync(
        Guid id,
        UpdateShiftDto request,
        IShiftService shiftService,
        CancellationToken cancellationToken)
    {
        try
        {
            var shift = await shiftService.UpdateAsync(id, request, cancellationToken);
            return shift is null ? Results.NotFound() : Results.Ok(shift);
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
    /// Deletes an existing shift.
    /// </summary>
    private static async Task<IResult> DeleteAsync(Guid id, IShiftService shiftService, CancellationToken cancellationToken)
    {
        var deleted = await shiftService.DeleteAsync(id, cancellationToken);
        return deleted ? Results.NoContent() : Results.NotFound();
    }

    /// <summary>
    /// Builds a consistent validation response payload.
    /// </summary>
    private static IResult CreateValidationProblem(string errorMessage)
    {
        return EndpointValidationResults.Create("shift", errorMessage);
    }
}
