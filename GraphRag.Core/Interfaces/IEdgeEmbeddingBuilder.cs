using GraphRag.Core.Models;


namespace GraphRag.Core.Interfaces;

public interface IEdgeEmbeddingBuilder
{
    Task<VectorDocument> BuildAsync(GraphEdgeContext edge, CancellationToken cancellationToken = default);
}
