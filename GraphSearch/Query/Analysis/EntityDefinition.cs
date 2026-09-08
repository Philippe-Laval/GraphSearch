namespace GraphSearch.Library.Query.Analysis;

/// <summary>
/// Entity definition represents a named entity extracted from a query, 
/// along with its type (e.g., person, location, organization).
/// </summary>
/// <param name="Name">The name of the entity.</param>
/// <param name="Type">The type of the entity.</param>
public sealed record EntityDefinition(
    string Name,
    string Type);