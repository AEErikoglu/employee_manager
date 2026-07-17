using backend.Dtos;
using backend.Services;

namespace backend.Endpoints;

/// <summary>
/// Registers invitation-related minimal API endpoints.
/// </summary>
public static class InvitationEndpoints
{
    /// <summary>
    /// Maps CRUD endpoints for invitations.
    /// </summary>
    public static IEndpointRouteBuilder MapInvitationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/invitations")
            .WithTags("Invitations");

        group.MapGet("/", GetAllAsync)
            .WithName("GetAllInvitations");

        group.MapGet("/{id:guid}", GetByIdAsync)
            .WithName("GetInvitationById");

        group.MapPost("/", CreateAsync)
            .WithName("CreateInvitation");

        group.MapPut("/{id:guid}", UpdateAsync)
            .WithName("UpdateInvitation");

        group.MapDelete("/{id:guid}", DeleteAsync)
            .WithName("DeleteInvitation");

        return endpoints;
    }

    /// <summary>
    /// Returns all invitations.
    /// </summary>
    private static async Task<IResult> GetAllAsync(IInvitationService invitationService, CancellationToken cancellationToken)
    {
        var invitations = await invitationService.GetAllAsync(cancellationToken);
        return Results.Ok(invitations);
    }

    /// <summary>
    /// Returns a single invitation by identifier.
    /// </summary>
    private static async Task<IResult> GetByIdAsync(Guid id, IInvitationService invitationService, CancellationToken cancellationToken)
    {
        var invitation = await invitationService.GetByIdAsync(id, cancellationToken);
        return invitation is null ? Results.NotFound() : Results.Ok(invitation);
    }

    /// <summary>
    /// Creates a new invitation.
    /// </summary>
    private static async Task<IResult> CreateAsync(
        CreateInvitationDto request,
        IInvitationService invitationService,
        CancellationToken cancellationToken)
    {
        try
        {
            var invitation = await invitationService.CreateAsync(request, cancellationToken);
            return Results.Created($"/invitations/{invitation.Id}", invitation);
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
    /// Updates an existing invitation.
    /// </summary>
    private static async Task<IResult> UpdateAsync(
        Guid id,
        UpdateInvitationDto request,
        IInvitationService invitationService,
        CancellationToken cancellationToken)
    {
        try
        {
            var invitation = await invitationService.UpdateAsync(id, request, cancellationToken);
            return invitation is null ? Results.NotFound() : Results.Ok(invitation);
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
    /// Deletes an existing invitation.
    /// </summary>
    private static async Task<IResult> DeleteAsync(Guid id, IInvitationService invitationService, CancellationToken cancellationToken)
    {
        var deleted = await invitationService.DeleteAsync(id, cancellationToken);
        return deleted ? Results.NoContent() : Results.NotFound();
    }

    /// <summary>
    /// Builds a consistent validation response payload.
    /// </summary>
    private static IResult CreateValidationProblem(string errorMessage)
    {
        return EndpointValidationResults.Create("invitation", errorMessage);
    }
}
