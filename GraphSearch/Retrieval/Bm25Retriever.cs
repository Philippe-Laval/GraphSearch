using System;
using System.Collections.Generic;
using System.Text;

namespace GraphSearch.Library.Retrieval
{
    public class Bm25Retriever : IBM25Retriever
    {
        Task<object> IBM25Retriever.SearchAsync(string query, int v, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
