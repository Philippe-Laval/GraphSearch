using GraphSearch.Library.Query;
using GraphSearch.Library.Ranking;

namespace GraphSearch.Library.Retrieval
{
    public sealed class GraphRetriever : IGraphRetriever
    {
        private readonly IVectorRetriever _vectorRetriever;
        private readonly IBM25Retriever _bm25Retriever;
        private readonly IGraphExpander _graphExpander;
        private readonly IGraphRanker _graphRanker;
        private readonly ISubgraphBuilder _subgraphBuilder;
        private readonly IRRFFusion _rrfFusion;

        public GraphRetriever(
            IVectorRetriever vectorRetriever,
            IBM25Retriever bm25Retriever,
            IGraphExpander graphExpander,
            IGraphRanker graphRanker,
            ISubgraphBuilder subgraphBuilder,
            IRRFFusion rrfFusion)
        {
            _vectorRetriever = vectorRetriever;
            _bm25Retriever = bm25Retriever;
            _graphExpander = graphExpander;
            _graphRanker = graphRanker;
            _subgraphBuilder = subgraphBuilder;
            _rrfFusion = rrfFusion;
        }

        public async Task<GraphRagResult> RetrieveAsync(
            string query,
            int topK,
            CancellationToken cancellationToken = default)
        {
            // 1. Semantic retrieval
            var vectorResults =
                await _vectorRetriever.SearchAsync(
                    query,
                    20,
                    cancellationToken);

            // 2. BM25 retrieval
            var lexicalResults =
                await _bm25Retriever.SearchAsync(
                    query,
                    20,
                    cancellationToken);

            // 3. Fuse results
            var seeds = _rrfFusion.Combine(
                vectorResults,
                lexicalResults);

            // 4. Expand graph around seeds
            var expanded =
                await _graphExpander.ExpandAsync(
                    seeds.Take(10),
                    maxDepth: 2,
                    cancellationToken);

            // 5. Graph ranking
            var ranked =
                await _graphRanker.RankAsync(
                    query,
                    seeds,
                    expanded,
                    cancellationToken);

            // 6. Select final subgraph
            return _subgraphBuilder.Build(
                ranked.Take(topK));
        }
    }
}
