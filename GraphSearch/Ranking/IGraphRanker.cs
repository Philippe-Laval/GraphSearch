using System;
using System.Collections.Generic;
using System.Text;

namespace GraphSearch.Library.Ranking
{
    public interface IGraphRanker
    {
        Task<IList<object>> RankAsync(string query, IList<object> seeds, object expanded, CancellationToken cancellationToken);
    }
}
