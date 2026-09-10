using GraphSearch.Library.Query;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphSearch.Library.Retrieval
{
    public interface IGraphRetriever
    {
        Task<GraphRagResult> RetrieveAsync(
            string query,
            int topK,
            CancellationToken cancellationToken = default);
    }
}
