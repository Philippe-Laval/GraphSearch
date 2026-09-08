namespace GraphSearch.Library.Query.Analysis.Model;

/*
For:
   
   "What is Microsoft's relationship with .NET 10?"
   
   you might get:
   
   Microsoft
       Type = Organization
       Confidence = 0.98
   
   .NET 10
       Type = Technology
       Confidence = 0.96
 */

/// <summary>
/// ExtractedEntity represents an entity extracted from a user's query,
/// including its text, type, position, length, and confidence score.
/// </summary>
/// <param name="Text">The text of the extracted entity.</param>
/// <param name="Type">The type of the extracted entity (e.g., Organization, Technology).</param>
/// <param name="Start">The starting position of the entity in the query.</param>
/// <param name="Length">The length of the entity text.</param>
/// <param name="Confidence">The confidence score of the entity extraction (in interval [0.0 - 1.0]).</param>
public sealed record ExtractedEntity(
    string Text,
    string? Type,
    int Start,
    int Length,
    double Confidence);