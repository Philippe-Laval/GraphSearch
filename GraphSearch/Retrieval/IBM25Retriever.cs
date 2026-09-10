namespace GraphSearch.Library.Retrieval
{
    public interface IBM25Retriever
    {
        Task<object> SearchAsync(string query, int v, CancellationToken cancellationToken);
    }
}