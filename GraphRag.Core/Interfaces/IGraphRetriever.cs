using GraphRag.Core.Models;

namespace GraphRag.Core.Interfaces;

public interface IGraphRetriever
{
    /// <summary>
    /// Retrieves a subgraph from the knowledge graph based on a natural language query.
    /// </summary>
    /// <param name="naturalLanguageQuery">The natural language query to use for retrieving the subgraph.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the retrieved subgraph.</returns>
    Task<SubGraph> RetrieveAsync(string naturalLanguageQuery);
}
