using GraphRag.Core.Models;

namespace GraphRag.AI.KnowledgeExtraction
{
    public interface IKnowledgeGraphSimpleExtractor
    {
        Task<KnowledgeGraph> ExtractAsync(string text, CancellationToken cancellationToken = default);
    }
}