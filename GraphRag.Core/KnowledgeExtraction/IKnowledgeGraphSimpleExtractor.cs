using GraphRag.Core.KnowledgeExtraction;

namespace GrapRag.Core.KnowledgeExtraction
{
    public interface IKnowledgeGraphSimpleExtractor
    {
        Task<KnowledgeGraph> ExtractAsync(string text, CancellationToken cancellationToken = default);
    }
}