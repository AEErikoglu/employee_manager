using System.Security.Claims;
using backend.Dtos;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

/// <summary>
/// Implements user-scoped workspace dashboard operations.
/// </summary>
public sealed class WorkspaceDashboardService(EmployeeManagerDbContext dbContext) : IWorkspaceDashboardService
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<WorkspaceDashboardDto>> GetWorkspacesAsync(
        ClaimsPrincipal principal,
        CancellationToken cancellationToken = default)
    {
        var appUser = await GetOrCreateAppUserAsync(principal, cancellationToken);

        return await dbContext.WorkplaceMembers
            .AsNoTracking()
            .Where(member => member.UserId == appUser.Id)
            .OrderBy(member => member.Workplace.Name)
            .Select(member => new WorkspaceDashboardDto
            {
                Id = member.WorkplaceId,
                Name = member.Workplace.Name,
                EmployeeCount = member.Workplace.WorkplaceMembers.Count(workplaceMember =>
                    workplaceMember.Role == WorkplaceRole.Employee),
                Role = member.Role.ToString(),
            })
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<WorkspaceDashboardDto> CreateWorkspaceAsync(
        ClaimsPrincipal principal,
        CreateDashboardWorkspaceDto request,
        CancellationToken cancellationToken = default)
    {
        var appUser = await GetOrCreateAppUserAsync(principal, cancellationToken);
        var name = NormalizeName(request.Name);
        var workplace = new Workplace
        {
            Id = Guid.NewGuid(),
            Name = name,
            CreatedByUserId = appUser.Id,
        };

        dbContext.Workplaces.Add(workplace);
        dbContext.WorkplaceMembers.Add(new WorkplaceMember
        {
            Id = Guid.NewGuid(),
            WorkplaceId = workplace.Id,
            UserId = appUser.Id,
            Role = WorkplaceRole.Manager,
        });

        await dbContext.SaveChangesAsync(cancellationToken);

        return new WorkspaceDashboardDto
        {
            Id = workplace.Id,
            Name = workplace.Name,
            EmployeeCount = 0,
            Role = WorkplaceRole.Manager.ToString(),
        };
    }

    /// <summary>
    /// Resolves the local user record and creates it on the first authenticated request.
    /// </summary>
    private async Task<AppUser> GetOrCreateAppUserAsync(ClaimsPrincipal principal, CancellationToken cancellationToken)
    {
        var externalAuthUserId = GetClaim(principal, "sub", ClaimTypes.NameIdentifier);
        var email = GetClaim(principal, "email", ClaimTypes.Email);

        if (string.IsNullOrWhiteSpace(externalAuthUserId) || string.IsNullOrWhiteSpace(email))
        {
            throw new UnauthorizedAccessException("The Supabase token must contain sub and email claims.");
        }

        var existingUser = await dbContext.AppUsers
            .SingleOrDefaultAsync(user => user.ExternalAuthUserId == externalAuthUserId, cancellationToken);

        if (existingUser is not null)
        {
            return existingUser;
        }

        var appUser = new AppUser
        {
            Id = Guid.NewGuid(),
            ExternalAuthUserId = externalAuthUserId,
            AuthProvider = "supabase",
            Email = email,
        };

        dbContext.AppUsers.Add(appUser);
        await dbContext.SaveChangesAsync(cancellationToken);

        return appUser;
    }

    /// <summary>
    /// Gets a JWT claim regardless of whether ASP.NET mapped its standard claim type.
    /// </summary>
    private static string? GetClaim(ClaimsPrincipal principal, string jwtClaimType, string mappedClaimType)
    {
        return principal.FindFirst(jwtClaimType)?.Value ?? principal.FindFirst(mappedClaimType)?.Value;
    }

    /// <summary>
    /// Validates and normalizes a workspace name.
    /// </summary>
    private static string NormalizeName(string name)
    {
        var normalizedName = name.Trim();

        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            throw new ArgumentException("Workspace name is required.", nameof(name));
        }

        if (normalizedName.Length > 200)
        {
            throw new ArgumentException("Workspace name must not exceed 200 characters.", nameof(name));
        }

        return normalizedName;
    }
}
