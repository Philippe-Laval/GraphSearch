using GraphRag.Lucene.Data;
using GraphRag.Lucene.Interfaces;
using GraphRag.Lucene.Models;
using System.Diagnostics;

namespace GraphRag.Lucene.Tests
{
    [TestClass]
    public sealed class LuceneTest
    {
        [TestMethod]
        public void TestSearchingAndResolvingResults()
        {
            var dataDirectory = Path.Combine(
                AppContext.BaseDirectory,
                "data");

            // string indexPath = Path.Combine(dataDirectory, "lucene");
            // string dataBasePath = Path.Combine(dataDirectory, "content.db");
            string indexPath = @"C:\LuceneIndex";
            string dataBasePath = @"C:\ChunkDB\content.db";
            string rootPath = @"C:\KnowledgeBase";

            IChunkRepository repository = new ChunkRepository(dataBasePath);

            repository.Initialize();


            using IChunkSearchIndex index = new ChunkSearchIndex(indexPath);

            var ingestor = new MarkdownIngestor(
                rootPath,
                repository);

            // Markdown -> SQLite and indexing queue.
            var changedDocuments = ingestor.IngestAll();

            // SQLite queue -> Lucene.
            var queueProcessor =
                new LuceneQueueProcessor(repository, index);

            // Process pending documents in batches until the queue is empty.
            int processed = 0;
            do
            {
                processed = queueProcessor.ProcessPending();
                Debug.WriteLine($"Processed {processed} documents.");
            } while (processed > 0);

            Console.WriteLine("Search results for various terms:\n");

            Console.WriteLine("Searching for 'Qualiparc':");
            SearchTerm("Qualiparc", repository, index);

            Console.WriteLine("\nSearching for 'DDM':");
            SearchTerm("DDM", repository, index);

            // OR operator is the default for multiple terms,
            // so this is equivalent to searching for 'Qualiparc OR DDM'.
            Console.WriteLine("\nSearching for 'Qualiparc DDM':");
            SearchTerm("Qualiparc DDM", repository, index);

            Console.WriteLine("\nSearching for 'Qualiparc AND DDM':");
            SearchTerm("Qualiparc AND DDM", repository, index);

            Console.WriteLine("\nSearching for 'Qualiparc OR DDM':");
            SearchTerm("Qualiparc OR DDM", repository, index);

            Console.WriteLine("\nSearching for 'tool':");
            SearchTerm("tool", repository, index);
        }

        private static void SearchTerm(string searchText, 
            IChunkRepository repository, 
            IChunkSearchIndex index)
        {
            // Search for documents containing the specified text.
            IReadOnlyList<ChunkSearchHit> searchHits = index.Search(
                searchText: searchText,
                maximumResults: 20);

            // Load canonical chunks from SQLite.
            var chunksById = repository
                .GetChunks(searchHits.Select(hit => hit.ChunkId))
                .ToDictionary(chunk => chunk.Id);

            var resolvedResults = searchHits
                .Where(hit => chunksById.ContainsKey(hit.ChunkId))
                .Select(hit => new
                {
                    hit.Score,
                    Chunk = chunksById[hit.ChunkId]
                })
                .ToArray();

            foreach (var result in resolvedResults)
            {
                Console.WriteLine(
                    $"{result.Score:F3} | " +
                    $"{result.Chunk.DocumentId} | " +
                    $"{result.Chunk.Text[..Math.Min(150, result.Chunk.Text.Length)]}");
            }
        }
    }
}
