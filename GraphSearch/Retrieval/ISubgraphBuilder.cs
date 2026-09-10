using GraphSearch.Library.Query;

namespace GraphSearch.Library.Retrieval
{
    public interface ISubgraphBuilder
    {
        GraphRagResult Build(IEnumerable<object> enumerable);
    }
}