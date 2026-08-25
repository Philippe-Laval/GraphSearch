namespace GraphSearch.Query.Analysis;

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
    Task<IReadOnlyList<ExtractedEntity>> ExtractAsync(
        string query,
        CancellationToken cancellationToken = default);
}