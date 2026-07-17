using backend.Dtos;
using backend.Services;

namespace backend.Endpoints;

/// <summary>
/// Registers application-user-related minimal API endpoints.
/// </summary>
public static class AppUserEndpoints
{
    /// <summary>
    /// Maps CRUD endpoints for application users.
    /// </summary>
    public static IEndpointRouteBuilder MapAppUserEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/app-users")
            .WithTags("AppUsers");

        group.MapGet("/", GetAllAsync)
            .WithName("GetAllAppUsers");

        group.MapGet("/{id:guid}", GetByIdAsync)
            .WithName("GetAppUserById");

        group.MapPost("/", CreateAsync)
            .WithName("CreateAppUser");

        group.MapPut("/{id:guid}", UpdateAsync)
            .WithName("UpdateAppUser");

        group.MapDelete("/{id:guid}", DeleteAsync)
            .WithName("DeleteAppUser");

        return endpoints;
    }

    /// <summary>
    /// Returns all application users.
    /// </summary>
    private static async Task<IResult> GetAllAsync(IAppUserService appUserService, CancellationToken cancellationToken)
    {
        var appUsers = await appUserService.GetAllAsync(cancellationToken);
        return Results.Ok(appUsers);
    }

    /// <summary>
    /// Returns a single application user by identifier.
    /// </summary>
    private static async Task<IResult> GetByIdAsync(Guid id, IAppUserService appUserService, CancellationToken cancellationToken)
    {
        var appUser = await appUserService.GetByIdAsync(id, cancellationToken);
        return appUser is null ? Results.NotFound() : Results.Ok(appUser);
    }

    /// <summary>
    /// Creates a new application user.
    /// </summary>
    private static async Task<IResult> CreateAsync(
        CreateAppUserDto request,
        IAppUserService appUserService,
        CancellationToken cancellationToken)
    {
        try
        {
            var appUser = await appUserService.CreateAsync(request, cancellationToken);
            return Results.Created($"/app-users/{appUser.Id}", appUser);
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
    /// Updates an existing application user.
    /// </summary>
    private static async Task<IResult> UpdateAsync(
        Guid id,
        UpdateAppUserDto request,
        IAppUserService appUserService,
        CancellationToken cancellationToken)
    {
        try
        {
            var appUser = await appUserService.UpdateAsync(id, request, cancellationToken);
            return appUser is null ? Results.NotFound() : Results.Ok(appUser);
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
    /// Deletes an existing application user.
    /// </summary>
    private static async Task<IResult> DeleteAsync(Guid id, IAppUserService appUserService, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await appUserService.DeleteAsync(id, cancellationToken);
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
        return EndpointValidationResults.Create("appUser", errorMessage);
    }
}
