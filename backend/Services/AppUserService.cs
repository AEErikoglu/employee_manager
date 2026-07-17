using backend.Dtos;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

/// <summary>
/// Implements CRUD operations for application users.
/// </summary>
public class AppUserService(EmployeeManagerDbContext dbContext) : IAppUserService
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<AppUserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.AppUsers
            .AsNoTracking()
            .OrderBy(x => x.CreatedAt)
            .Select(MapToDto())
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<AppUserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.AppUsers
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(MapToDto())
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<AppUserDto> CreateAsync(CreateAppUserDto request, CancellationToken cancellationToken = default)
    {
        var externalAuthUserId = NormalizeRequired(request.ExternalAuthUserId, 200, "External auth user id");
        var authProvider = NormalizeRequired(request.AuthProvider, 50, "Auth provider");
        var email = NormalizeRequired(request.Email, 255, "Email");
        var displayName = NormalizeOptional(request.DisplayName, 200, "Display name");

        var exists = await dbContext.AppUsers
            .AsNoTracking()
            .AnyAsync(x => x.ExternalAuthUserId == externalAuthUserId, cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException($"An app user with external auth user id '{externalAuthUserId}' already exists.");
        }

        var appUser = new AppUser
        {
            Id = Guid.NewGuid(),
            ExternalAuthUserId = externalAuthUserId,
            AuthProvider = authProvider,
            Email = email,
            DisplayName = displayName
        };

        dbContext.AppUsers.Add(appUser);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(appUser);
    }

    /// <inheritdoc />
    public async Task<AppUserDto?> UpdateAsync(Guid id, UpdateAppUserDto request, CancellationToken cancellationToken = default)
    {
        var appUser = await dbContext.AppUsers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (appUser is null)
        {
            return null;
        }

        var externalAuthUserId = NormalizeRequired(request.ExternalAuthUserId, 200, "External auth user id");
        var authProvider = NormalizeRequired(request.AuthProvider, 50, "Auth provider");
        var email = NormalizeRequired(request.Email, 255, "Email");
        var displayName = NormalizeOptional(request.DisplayName, 200, "Display name");

        var exists = await dbContext.AppUsers
            .AsNoTracking()
            .AnyAsync(x => x.ExternalAuthUserId == externalAuthUserId && x.Id != id, cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException($"An app user with external auth user id '{externalAuthUserId}' already exists.");
        }

        appUser.ExternalAuthUserId = externalAuthUserId;
        appUser.AuthProvider = authProvider;
        appUser.Email = email;
        appUser.DisplayName = displayName;

        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(appUser);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var appUser = await dbContext.AppUsers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (appUser is null)
        {
            return false;
        }

        var hasCreatedWorkplaces = await dbContext.Workplaces
            .AsNoTracking()
            .AnyAsync(x => x.CreatedByUserId == id, cancellationToken);

        if (hasCreatedWorkplaces)
        {
            throw new InvalidOperationException("The app user cannot be deleted because the user still owns workplaces.");
        }

        dbContext.AppUsers.Remove(appUser);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            throw new InvalidOperationException("The app user cannot be deleted because related records still depend on it.");
        }

        return true;
    }

    /// <summary>
    /// Validates and normalizes a required string value.
    /// </summary>
    private static string NormalizeRequired(string? value, int maxLength, string fieldName)
    {
        var normalizedValue = value?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(normalizedValue))
        {
            throw new ArgumentException($"{fieldName} is required.", fieldName);
        }

        if (normalizedValue.Length > maxLength)
        {
            throw new ArgumentException($"{fieldName} must not exceed {maxLength} characters.", fieldName);
        }

        return normalizedValue;
    }

    /// <summary>
    /// Validates and normalizes an optional string value.
    /// </summary>
    private static string? NormalizeOptional(string? value, int maxLength, string fieldName)
    {
        var normalizedValue = value?.Trim();

        if (string.IsNullOrWhiteSpace(normalizedValue))
        {
            return null;
        }

        if (normalizedValue.Length > maxLength)
        {
            throw new ArgumentException($"{fieldName} must not exceed {maxLength} characters.", fieldName);
        }

        return normalizedValue;
    }

    /// <summary>
    /// Builds a projection from the entity to the API response DTO.
    /// </summary>
    private static System.Linq.Expressions.Expression<Func<AppUser, AppUserDto>> MapToDto()
    {
        return appUser => new AppUserDto
        {
            Id = appUser.Id,
            ExternalAuthUserId = appUser.ExternalAuthUserId,
            AuthProvider = appUser.AuthProvider,
            Email = appUser.Email,
            DisplayName = appUser.DisplayName,
            CreatedAt = appUser.CreatedAt
        };
    }

    /// <summary>
    /// Maps an already loaded entity to the API response DTO.
    /// </summary>
    private static AppUserDto MapToDto(AppUser appUser)
    {
        return new AppUserDto
        {
            Id = appUser.Id,
            ExternalAuthUserId = appUser.ExternalAuthUserId,
            AuthProvider = appUser.AuthProvider,
            Email = appUser.Email,
            DisplayName = appUser.DisplayName,
            CreatedAt = appUser.CreatedAt
        };
    }
}
