namespace GraphSearch.Query.Analysis;

public interface IQueryAnalyzer
{
    Task<string> NormalizeAsync(
        string query,
        CancellationToken cancellationToken = default);
}