namespace GraphRag.Core.Models;

/// <summary>
/// Represents metadata information about a graph, including schema details, 
/// generation information, and graph statistics.
/// </summary>
public sealed record Metadata
{
    /// <summary>
    /// Gets the schema information for the graph, 
    /// which defines the structure and organization of the graph data.
    /// </summary>
    public required string Schema { get; init; }

    /// <summary>
    /// Gets the version of the generator that created the graph, 
    /// which can be used to track changes or updates to the graph generation process.
    /// </summary>
    public required string GeneratorVersion { get; init; }

    /// <summary>
    /// Get the document type, which can be used to identify the type of document 
    /// or data being represented in the graph.
    /// </summary>
    public required string DocumentType { get; init; }

    /// <summary>
    /// Gets the graph identifier.
    /// </summary>
    public required string GraphId { get; init; }
    
    /// <summary>
    /// Gets the UTC date and time when the graph was generated.
    /// </summary>
    public required DateTime GeneratedAtUtc { get; init; }

    /// <summary>
    /// EntityHash is a unique identifier for the graph entity, 
    /// which can be used to track changes or updates to the graph over time.
    /// </summary>
    public required string EntityHash { get; init; }

    /// <summary>
    /// Node count is the total number of nodes in the graph, 
    /// which can be used to understand the size and complexity of the graph.
    /// </summary>
    public required int NodeCount { get; init; }

    /// <summary>
    /// Edge count is the total number of edges in the graph, 
    /// which can be used to understand the connectivity and relationships within the graph.
    /// </summary>
    public required int EdgeCount { get; init; }

    /// <summary>
    /// Culture represents the cultural or regional context of the graph, 
    /// which can be used to understand localization or language-specific aspects of the graph.
    /// </summary>
    public required string Culture { get; init; }
}
