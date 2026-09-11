using GraphRag.Core.Models;
using System.Reflection.Metadata;

namespace GraphRag.Core.Interfaces
{
    public interface ILlmReranker
    {
        Task<RerankerAnalysis> ReRankAsync(string query, string document, CancellationToken cancellationToken = default);
    }
}