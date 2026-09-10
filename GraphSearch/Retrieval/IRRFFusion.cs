namespace GraphSearch.Library.Retrieval
{
    public interface IRRFFusion
    {
        IList<object> Combine(object vectorResults, object lexicalResults);
    }
}