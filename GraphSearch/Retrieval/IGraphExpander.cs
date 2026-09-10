namespace GraphSearch.Library.Retrieval
{
    public interface IGraphExpander
    {
        Task<object> ExpandAsync(IEnumerable<object> enumerable, int maxDepth, CancellationToken cancellationToken);
    }
}