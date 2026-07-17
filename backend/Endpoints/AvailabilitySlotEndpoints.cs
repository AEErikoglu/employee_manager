using backend.Dtos;
using backend.Services;

namespace backend.Endpoints;

/// <summary>
/// Registers availability-slot-related minimal API endpoints.
/// </summary>
public static class AvailabilitySlotEndpoints
{
    /// <summary>
    /// Maps CRUD endpoints for availability slots.
    /// </summary>
    public static IEndpointRouteBuilder MapAvailabilitySlotEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/availability-slots")
            .WithTags("AvailabilitySlots");

        group.MapGet("/", GetAllAsync)
            .WithName("GetAllAvailabilitySlots");

        group.MapGet("/{id:guid}", GetByIdAsync)
            .WithName("GetAvailabilitySlotById");

        group.MapPost("/", CreateAsync)
            .WithName("CreateAvailabilitySlot");

        group.MapPut("/{id:guid}", UpdateAsync)
            .WithName("UpdateAvailabilitySlot");

        group.MapDelete("/{id:guid}", DeleteAsync)
            .WithName("DeleteAvailabilitySlot");

        return endpoints;
    }

    /// <summary>
    /// Returns all availability slots.
    /// </summary>
    private static async Task<IResult> GetAllAsync(IAvailabilitySlotService availabilitySlotService, CancellationToken cancellationToken)
    {
        var availabilitySlots = await availabilitySlotService.GetAllAsync(cancellationToken);
        return Results.Ok(availabilitySlots);
    }

    /// <summary>
    /// Returns a single availability slot by identifier.
    /// </summary>
    private static async Task<IResult> GetByIdAsync(Guid id, IAvailabilitySlotService availabilitySlotService, CancellationToken cancellationToken)
    {
        var availabilitySlot = await availabilitySlotService.GetByIdAsync(id, cancellationToken);
        return availabilitySlot is null ? Results.NotFound() : Results.Ok(availabilitySlot);
    }

    /// <summary>
    /// Creates a new availability slot.
    /// </summary>
    private static async Task<IResult> CreateAsync(
        CreateAvailabilitySlotDto request,
        IAvailabilitySlotService availabilitySlotService,
        CancellationToken cancellationToken)
    {
        try
        {
            var availabilitySlot = await availabilitySlotService.CreateAsync(request, cancellationToken);
            return Results.Created($"/availability-slots/{availabilitySlot.Id}", availabilitySlot);
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
    /// Updates an existing availability slot.
    /// </summary>
    private static async Task<IResult> UpdateAsync(
        Guid id,
        UpdateAvailabilitySlotDto request,
        IAvailabilitySlotService availabilitySlotService,
        CancellationToken cancellationToken)
    {
        try
        {
            var availabilitySlot = await availabilitySlotService.UpdateAsync(id, request, cancellationToken);
            return availabilitySlot is null ? Results.NotFound() : Results.Ok(availabilitySlot);
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
    /// Deletes an existing availability slot.
    /// </summary>
    private static async Task<IResult> DeleteAsync(Guid id, IAvailabilitySlotService availabilitySlotService, CancellationToken cancellationToken)
    {
        var deleted = await availabilitySlotService.DeleteAsync(id, cancellationToken);
        return deleted ? Results.NoContent() : Results.NotFound();
    }

    /// <summary>
    /// Builds a consistent validation response payload.
    /// </summary>
    private static IResult CreateValidationProblem(string errorMessage)
    {
        return EndpointValidationResults.Create("availabilitySlot", errorMessage);
    }
}
