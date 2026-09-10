using System;
using System.Collections.Generic;
using System.Text;

namespace GraphSearch.Library.Retrieval
{
    public class Bm25Retriever : IBM25Retriever
    {
        public Task<object> SearchAsync(string query, int v, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
