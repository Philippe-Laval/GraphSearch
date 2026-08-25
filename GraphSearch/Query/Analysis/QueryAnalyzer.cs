namespace GraphSearch.Query.Analysis;

/*
This looks trivial, but I'd keep it as a separate component because later 
you may want to do things like:
   
   "What technologies does Microsoft use?"
                │
                ▼
   "What technologies are used/developed by Microsoft?"
   
   or extract query intent.
 */

public sealed class QueryAnalyzer : IQueryAnalyzer
{
    public Task<string> NormalizeAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        var normalized = query
            .Trim()
            .ReplaceLineEndings(" ");

        return Task.FromResult(normalized);
    }
}