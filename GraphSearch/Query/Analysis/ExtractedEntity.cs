namespace GraphSearch.Query.Analysis;

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

public sealed record ExtractedEntity(
    string Text,
    string? Type,
    int Start,
    int Length,
    double Confidence);