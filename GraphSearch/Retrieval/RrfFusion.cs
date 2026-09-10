using System;
using System.Collections.Generic;
using System.Text;

namespace GraphSearch.Library.Retrieval
{
    public class RrfFusion : IRRFFusion
    {
        public static double RrfScore(
            int vectorRank,
            int bm25Rank,
            int k = 60)
        {
            return
                1.0 / (k + vectorRank) +
                1.0 / (k + bm25Rank);
        }

        public IList<object> Combine(object vectorResults, object lexicalResults)
        {
            throw new NotImplementedException();
        }
    }
}
