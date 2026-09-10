using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphSearch.Library.Retrieval
{
    public class GraphExpander : IGraphExpander
    {
        public Task<object> ExpandAsync(IEnumerable<object> enumerable, int maxDepth, CancellationToken cancellationToken)
        {
            return Task.FromResult((object)enumerable);
        }
    }
}
