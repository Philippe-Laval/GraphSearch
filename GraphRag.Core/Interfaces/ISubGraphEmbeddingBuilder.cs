using GraphRag.Core.Models;


namespace GraphRag.Core.Interfaces;

public interface ISubGraphEmbeddingBuilder
{
    Task<VectorDocument> BuildAsync(SubGraph2 graph, CancellationToken cancellationToken = default);
}
