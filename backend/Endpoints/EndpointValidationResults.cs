namespace backend.Endpoints;

/// <summary>
/// Creates consistent validation responses for minimal API endpoints.
/// </summary>
internal static class EndpointValidationResults
{
    /// <summary>
    /// Builds a validation problem result for a specific entity key.
    /// </summary>
    public static IResult Create(string key, string errorMessage)
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            [key] = [errorMessage]
        });
    }
}
