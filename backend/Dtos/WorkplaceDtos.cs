namespace backend.Dtos;

/// <summary>
/// Represents a workplace returned by the API.
/// </summary>
public class WorkplaceDto
{
    /// <summary>
    /// Gets or sets the workplace identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the workplace name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the user who created the workplace.
    /// </summary>
    public Guid CreatedByUserId { get; set; }

    /// <summary>
    /// Gets or sets when the workplace was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Represents the request payload for creating a workplace.
/// </summary>
public class CreateWorkplaceDto
{
    /// <summary>
    /// Gets or sets the workplace name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the user creating the workplace.
    /// </summary>
    public Guid CreatedByUserId { get; set; }
}

/// <summary>
/// Represents the request payload for updating a workplace.
/// </summary>
public class UpdateWorkplaceDto
{
    /// <summary>
    /// Gets or sets the updated workplace name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
