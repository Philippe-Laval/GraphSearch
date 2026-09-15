using GraphRag.BM25.Algorithms;
using GraphRag.BM25.Interfaces;
using GraphRag.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;
using System.Text;

namespace GraphRag.BM25.Retrievers;

public class HybridRetriever : IRetriever
{
    private readonly string _indexFolder;

    public HybridRetriever(string indexFolder)
    {
        _indexFolder = indexFolder;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<SearchResult>> RetrieveAsync(string query, int topK, CancellationToken cancellationToken = default)
    {
        /*
         A HybridRetriever would invoke these retrievers (potentially in parallel), 
        apply weighted RRF to their ranked results, and return the fused candidate list.
        This design keeps each retrieval strategy independent and makes it easy to 
        add future retrievers (such as metadata filtering or SQL retrieval) 
        without changing the fusion logic.
         */

        Bm25Retriever retriever1 = new Bm25Retriever(_indexFolder);
        var resultBM25 = await retriever1.RetrieveAsync(query, topK, cancellationToken);

        VectorRetriever retriever2 = new VectorRetriever();
        var resultVector = await retriever2.RetrieveAsync(query, topK, cancellationToken);

        GraphRetriever retriever3 = new GraphRetriever();
        var resultGraph = await retriever3.RetrieveAsync(query, topK, cancellationToken);

        // You can adjust the weight as needed
        double weightBM25 = 1.0; 
        double weightVector = 1.0;
        double weightGraph = 1.0;

        var input =
            new List<(double Weight, IReadOnlyList<SearchResult> Ranking)>
            {
                (weightBM25, resultBM25),
                (weightVector, resultVector),
                (weightGraph, resultGraph),
            };

        var results = ReciprocalRankFusion.FuseWeighted(input);

        return results;
    }
}
