namespace GraphRag.Core.Models;


/// <summary>
/// A node in a graph, representing an entity with a unique identifier, type, label, description, and additional properties.
/// </summary>
/// <param name="Id">The unique identifier of the node.</param>
/// <param name="Type">The type of the node.</param>
/// <param name="Label">The label of the node.</param>
/// <param name="Description">The description of the node.</param>
/// <param name="Properties">Additional properties of the node.</param>
public sealed record GraphNode(
    Guid Id,
    string Type,
    string Label,
    string Description,
    IReadOnlyDictionary<string, object?> Properties);
