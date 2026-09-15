using GraphRag.BM25.Interfaces;
using GraphRag.Core.Models;
using GraphRag.Lucene;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Search.Similarities;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System;
using System.Collections.Generic;
using System.Text;


namespace GraphRag.BM25.Retrievers;

/// <summary>
/// Uses the BM25 algorithm to retrieve relevant documents from a Lucene index based on a given query. This class implements the IRetriever interface and provides an asynchronous method to retrieve search results,
/// returning them as a list of SearchResult objects ordered by their relevance scores.
/// </summary>
public sealed class Bm25Retriever : IRetriever
{
    private readonly IndexSearcher _searcher;
    private readonly QueryParser _parser;

    public Bm25Retriever(string folder)
    {
        var directory = FSDirectory.Open(folder);

        var reader = DirectoryReader.Open(directory);

        _searcher = new IndexSearcher(reader);

        _searcher.Similarity = new BM25Similarity();

        _parser =
            new QueryParser(
                LuceneVersion.LUCENE_48,
                LuceneFields.Text,
                new StandardAnalyzer(
                    LuceneVersion.LUCENE_48));
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<SearchResult>> RetrieveAsync(
        string query,
        int topK,
        CancellationToken cancellationToken = default)
    {
        var luceneQuery =
            _parser.Parse(query);

        var topDocs =
            _searcher.Search(luceneQuery, topK);

        var results = new List<SearchResult>();

        foreach (var hit in topDocs.ScoreDocs)
        {
            var doc =
                _searcher.Doc(hit.Doc);

            string? chunkIdString = doc.Get(LuceneFields.ChunkId);
            if (chunkIdString == null)
            {
                continue;
            }

            results.Add(
                new SearchResult(int.Parse(chunkIdString), hit.Score));
        }

        var orderResult = results.OrderByDescending(r => r.Score).ToList();

        return Task.FromResult<IReadOnlyList<SearchResult>>(orderResult);
    }
}