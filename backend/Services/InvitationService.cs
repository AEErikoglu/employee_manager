using backend.Dtos;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

/// <summary>
/// Implements CRUD operations for invitations.
/// </summary>
public class InvitationService(EmployeeManagerDbContext dbContext) : IInvitationService
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<InvitationDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Invitations
            .AsNoTracking()
            .OrderBy(x => x.CreatedAt)
            .Select(MapToDto())
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<InvitationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Invitations
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(MapToDto())
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<InvitationDto> CreateAsync(CreateInvitationDto request, CancellationToken cancellationToken = default)
    {
        await ValidateInvitationAsync(
            request.WorkplaceId,
            request.InvitedEmail,
            request.InvitedRole,
            request.Token,
            request.Status,
            request.InvitedByMemberId,
            request.AcceptedByMemberId,
            null,
            cancellationToken);

        var invitation = new Invitation
        {
            Id = Guid.NewGuid(),
            WorkplaceId = request.WorkplaceId,
            InvitedEmail = NormalizeRequired(request.InvitedEmail, 255, "Invited email"),
            InvitedRole = request.InvitedRole,
            Token = NormalizeRequired(request.Token, 300, "Token"),
            Status = request.Status,
            InvitedByMemberId = request.InvitedByMemberId,
            AcceptedByMemberId = request.AcceptedByMemberId,
            ExpiresAt = request.ExpiresAt,
            AcceptedAt = request.AcceptedAt
        };

        dbContext.Invitations.Add(invitation);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(invitation);
    }

    /// <inheritdoc />
    public async Task<InvitationDto?> UpdateAsync(Guid id, UpdateInvitationDto request, CancellationToken cancellationToken = default)
    {
        var invitation = await dbContext.Invitations.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (invitation is null)
        {
            return null;
        }

        await ValidateInvitationAsync(
            request.WorkplaceId,
            request.InvitedEmail,
            request.InvitedRole,
            request.Token,
            request.Status,
            request.InvitedByMemberId,
            request.AcceptedByMemberId,
            id,
            cancellationToken);

        invitation.WorkplaceId = request.WorkplaceId;
        invitation.InvitedEmail = NormalizeRequired(request.InvitedEmail, 255, "Invited email");
        invitation.InvitedRole = request.InvitedRole;
        invitation.Token = NormalizeRequired(request.Token, 300, "Token");
        invitation.Status = request.Status;
        invitation.InvitedByMemberId = request.InvitedByMemberId;
        invitation.AcceptedByMemberId = request.AcceptedByMemberId;
        invitation.ExpiresAt = request.ExpiresAt;
        invitation.AcceptedAt = request.AcceptedAt;

        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(invitation);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var invitation = await dbContext.Invitations.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (invitation is null)
        {
            return false;
        }

        dbContext.Invitations.Remove(invitation);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <summary>
    /// Validates an invitation payload before create or update.
    /// </summary>
    private async Task ValidateInvitationAsync(
        Guid workplaceId,
        string invitedEmail,
        WorkplaceRole invitedRole,
        string token,
        InvitationStatus status,
        Guid invitedByMemberId,
        Guid? acceptedByMemberId,
        Guid? currentInvitationId,
        CancellationToken cancellationToken)
    {
        NormalizeRequired(invitedEmail, 255, "Invited email");
        NormalizeRequired(token, 300, "Token");

        if (!Enum.IsDefined(invitedRole))
        {
            throw new ArgumentException("The provided invited role is invalid.", nameof(invitedRole));
        }

        if (!Enum.IsDefined(status))
        {
            throw new ArgumentException("The provided invitation status is invalid.", nameof(status));
        }

        var workplaceExists = await dbContext.Workplaces
            .AsNoTracking()
            .AnyAsync(x => x.Id == workplaceId, cancellationToken);

        if (!workplaceExists)
        {
            throw new InvalidOperationException($"Workplace '{workplaceId}' was not found.");
        }

        var tokenExists = await dbContext.Invitations
            .AsNoTracking()
            .AnyAsync(x => x.Token == token && (!currentInvitationId.HasValue || x.Id != currentInvitationId.Value), cancellationToken);

        if (tokenExists)
        {
            throw new InvalidOperationException($"An invitation with token '{token}' already exists.");
        }

        var invitedByMember = await dbContext.WorkplaceMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == invitedByMemberId, cancellationToken);

        if (invitedByMember is null)
        {
            throw new InvalidOperationException($"Workplace member '{invitedByMemberId}' was not found.");
        }

        if (invitedByMember.WorkplaceId != workplaceId)
        {
            throw new InvalidOperationException("The inviting member must belong to the same workplace as the invitation.");
        }

        if (!acceptedByMemberId.HasValue)
        {
            return;
        }

        var acceptedByMember = await dbContext.WorkplaceMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == acceptedByMemberId.Value, cancellationToken);

        if (acceptedByMember is null)
        {
            throw new InvalidOperationException($"Workplace member '{acceptedByMemberId}' was not found.");
        }

        if (acceptedByMember.WorkplaceId != workplaceId)
        {
            throw new InvalidOperationException("The accepted membership must belong to the same workplace as the invitation.");
        }
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
    /// Builds a projection from the entity to the API response DTO.
    /// </summary>
    private static System.Linq.Expressions.Expression<Func<Invitation, InvitationDto>> MapToDto()
    {
        return invitation => new InvitationDto
        {
            Id = invitation.Id,
            WorkplaceId = invitation.WorkplaceId,
            InvitedEmail = invitation.InvitedEmail,
            InvitedRole = invitation.InvitedRole,
            Token = invitation.Token,
            Status = invitation.Status,
            InvitedByMemberId = invitation.InvitedByMemberId,
            AcceptedByMemberId = invitation.AcceptedByMemberId,
            CreatedAt = invitation.CreatedAt,
            ExpiresAt = invitation.ExpiresAt,
            AcceptedAt = invitation.AcceptedAt
        };
    }

    /// <summary>
    /// Maps an already loaded entity to the API response DTO.
    /// </summary>
    private static InvitationDto MapToDto(Invitation invitation)
    {
        return new InvitationDto
        {
            Id = invitation.Id,
            WorkplaceId = invitation.WorkplaceId,
            InvitedEmail = invitation.InvitedEmail,
            InvitedRole = invitation.InvitedRole,
            Token = invitation.Token,
            Status = invitation.Status,
            InvitedByMemberId = invitation.InvitedByMemberId,
            AcceptedByMemberId = invitation.AcceptedByMemberId,
            CreatedAt = invitation.CreatedAt,
            ExpiresAt = invitation.ExpiresAt,
            AcceptedAt = invitation.AcceptedAt
        };
    }
}
