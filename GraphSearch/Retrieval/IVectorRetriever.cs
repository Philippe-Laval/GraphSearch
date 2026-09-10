namespace GraphSearch.Library.Retrieval
{
    public interface IVectorRetriever
    {
        Task<object> SearchAsync(string query, int v, CancellationToken cancellationToken);
    }
}