namespace GraphSearch.Library.Query.Analysis.EntityExtraction;

/*
I'd define the interface so the implementation could be:
   
   regex/rules
   dictionary
   NER model
   LLM
   hybrid
 */

public interface IEntityExtractor
{
    /// <summary>
    /// Extracts entities from the given query string and returns a list of extracted entities.
    /// </summary>
    /// <param name="query">The query string from which to extract entities.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of extracted entities.</returns>
    Task<IReadOnlyList<ExtractedEntity>> ExtractAsync(
        string query,
        CancellationToken cancellationToken = default);
}