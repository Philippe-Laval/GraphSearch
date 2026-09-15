using GraphRag.BM25.Algorithms;
using GraphRag.Core.Models;

namespace GraphRag.BM25.Tests
{
    [TestClass]
    public sealed class ReciprocalRankFusionTest
    {
        [TestMethod]
        public void TestFuse()
        {
            var bm25 = new List<SearchResult>
            {
                new(1,18.2),
                new(2,12.1),
                new(3,4.8)
            };

            var vector = new List<SearchResult>
            {
                new(3,0.93),
                new(1,0.89),
                new(4,0.87)
            };

            var results = ReciprocalRankFusion.Fuse(bm25, vector);

            foreach (var r in results)
            {
                Console.WriteLine($"{r.ChunkId} {r.Score}");
            }

        }
    }
}
