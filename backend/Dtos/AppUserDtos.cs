namespace backend.Dtos;

/// <summary>
/// Represents an application user returned by the API.
/// </summary>
public class AppUserDto
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the external authentication user identifier.
    /// </summary>
    public string ExternalAuthUserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the authentication provider name.
    /// </summary>
    public string AuthProvider { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional display name.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Gets or sets when the user was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Represents the request payload for creating an application user.
/// </summary>
public class CreateAppUserDto
{
    /// <summary>
    /// Gets or sets the external authentication user identifier.
    /// </summary>
    public string ExternalAuthUserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the authentication provider name.
    /// </summary>
    public string AuthProvider { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional display name.
    /// </summary>
    public string? DisplayName { get; set; }
}

/// <summary>
/// Represents the request payload for updating an application user.
/// </summary>
public class UpdateAppUserDto
{
    /// <summary>
    /// Gets or sets the external authentication user identifier.
    /// </summary>
    public string ExternalAuthUserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the authentication provider name.
    /// </summary>
    public string AuthProvider { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional display name.
    /// </summary>
    public string? DisplayName { get; set; }
}
