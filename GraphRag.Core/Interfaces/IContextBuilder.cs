using GraphRag.Core.Models;

namespace GraphRag.Core.Interfaces;

public interface IContextBuilder
{
    /// <summary>
    /// Build a context string from the given subgraph.
    /// </summary>
    /// <param name="graph">The subgraph to build the context from.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the context string.</returns>
    Task<string> BuildContextAsync(SubGraph graph);
}