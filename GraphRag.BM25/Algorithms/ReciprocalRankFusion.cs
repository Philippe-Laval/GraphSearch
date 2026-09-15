using GraphRag.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace GraphRag.BM25.Algorithms;

/// <summary>
/// Provides methods for combining multiple search result rankings using the Reciprocal Rank Fusion algorithm.
/// </summary>
public static class ReciprocalRankFusion
{
    /// <summary>
    /// Combines multiple search result rankings using the Reciprocal Rank Fusion algorithm.
    /// </summary>
    /// <param name="rankings">The search result rankings to combine.</param>
    /// <returns>A list of combined search results.</returns>
    public static IReadOnlyList<SearchResult> Fuse(
        params IReadOnlyList<SearchResult>[] rankings)
    {
        const double k = 60;

        var scores = new Dictionary<int, double>();

        foreach (var ranking in rankings)
        {
            var sortedRanking = ranking.OrderByDescending(x => x.Score);

            int rank = 1;

            foreach (var searchResult in sortedRanking)
            {
                var chunkId = searchResult.ChunkId;

                scores.TryAdd(chunkId, 0);

                scores[chunkId] +=
                    1.0 / (k + rank);

                rank++;
            }
        }

        return scores
            .OrderByDescending(x => x.Value)
            .Select(x => new SearchResult(
                x.Key,
                x.Value))
            .ToList();
    }

    /// <summary>
    /// Fuses multiple search rankings using weighted reciprocal rank fusion to produce a single ranked list of search
    /// results.
    /// </summary>
    /// <remarks>Uses a constant k value of 60 for the reciprocal rank fusion formula. Each result's score is
    /// calculated as the sum of weight/(k + rank) across all input rankings where it appears.</remarks>
    /// <param name="inputs">A collection of tuples containing a weight and its corresponding ranking of search results to be fused.</param>
    /// <returns>A fused ranking of search results ordered by their computed scores in descending order.</returns>
    public static IReadOnlyList<SearchResult> FuseWeighted(
    IEnumerable<(double Weight, IReadOnlyList<SearchResult> Ranking)> inputs)
    {
        const double k = 60;

        var scores = new Dictionary<int, double>();

        foreach (var (weight, ranking) in inputs)
        {
            var sortedRanking = ranking.OrderByDescending(x => x.Score);

            int rank = 1;

            foreach (var result in sortedRanking)
            {
                var chunkId = result.ChunkId;

                scores.TryAdd(chunkId, 0);

                scores[chunkId] +=
                    weight / (k + rank);

                rank++;
            }
        }

        return scores
            .OrderByDescending(x => x.Value)
            .Select(x => new SearchResult(
                x.Key,
                x.Value))
            .ToList();
    }
}
