namespace GraphSearch.Library.Query.Analysis.EntityExtraction;

/// <summary>
/// Shared helpers to merge overlapping <see cref="ExtractedEntity"/> spans
/// while preserving the highest-priority / highest-confidence match.
///
/// Best-practice policy for GraphRAG:
///   1. Prefer higher priority (extractor tier).
///   2. Break ties by higher confidence.
///   3. Break further ties by longer span (more specific wins).
/// </summary>
public static class EntitySpanMerger
{
    /// <summary>
    /// Adds <paramref name="candidate"/> to <paramref name="accumulator"/> unless
    /// it overlaps something with higher priority/confidence. If it dominates
    /// existing overlaps, they are removed.
    /// </summary>
    /// <param name="accumulator"></param>
    /// <param name="candidate"></param>
    /// <param name="priority"></param>
    public static void AddOrReplace(
        List<(ExtractedEntity Entity, int Priority)> accumulator,
        ExtractedEntity candidate,
        int priority)
    {
        for (var i = accumulator.Count - 1; i >= 0; i--)
        {
            var (existing, existingPriority) = accumulator[i];

            // If the candidate doesn't overlap with the existing entity, skip it.
            if (!Overlap(candidate, existing))
            {
                continue;
            }

            // If the candidate wins, remove the existing entity from the accumulator.
            if (Wins(candidate, priority, existing, existingPriority))
            {
                accumulator.RemoveAt(i);
            }
            else
            {
                // existing dominates; drop candidate
                return; 
            }
        }

        // If we reach here, the candidate is either non-overlapping or dominates all overlaps.
        accumulator.Add((candidate, priority));
    }

    /// <summary>
    /// Deterministic ordering: by start position, then by (negative) length so
    /// longer spans come first. Useful before returning results.
    /// </summary>
    public static IReadOnlyList<ExtractedEntity> Finalize(
        List<(ExtractedEntity Entity, int Priority)> accumulator)
    {
        return accumulator
            .Select(x => x.Entity)
            .OrderBy(e => e.Start)
            .ThenByDescending(e => e.Length)
            .ToList();
    }

    /// <summary>
    /// Détermine si les plages de deux entités extraites se chevauchent.
    /// </summary>
    /// <param name="a">Entité extraite à comparer.</param>
    /// <param name="b">Autre entité extraite à comparer.</param>
    /// <returns><see langword="true"/> si les plages se chevauchent ; sinon, <see langword="false"/>.</returns>
    private static bool Overlap(ExtractedEntity a, ExtractedEntity b)
    {
        var aEnd = a.Start + a.Length;
        var bEnd = b.Start + b.Length;
        return a.Start < bEnd && aEnd > b.Start;
    }

    /// <summary>
    /// Détermine si l'entité extraite <paramref name="a"/> l'emporte sur l'entité extraite <paramref name="b"/>
    /// en fonction de la priorité, de la confiance et de la longueur.
    /// </summary>
    /// <param name="a">Entité extraite à comparer.</param>
    /// <param name="aPriority">Priorité de l'entité extraite <paramref name="a"/>.</param>
    /// <param name="b">Autre entité extraite à comparer.</param>
    /// <param name="bPriority">Priorité de l'entité extraite <paramref name="b"/>.</param>
    /// <returns><see langword="true"/> si l'entité <paramref name="a"/> l'emporte ; sinon, <see langword="false"/>.</returns>
    private static bool Wins(
        ExtractedEntity a, int aPriority,
        ExtractedEntity b, int bPriority)
    {
        // Compare by priority first (higher wins)
        if (aPriority != bPriority)
        {
            return aPriority > bPriority;
        }

        // If priorities are equal, compare by confidence (higher wins)
        if (Math.Abs(a.Confidence - b.Confidence) > 1e-9)
        {
            return a.Confidence > b.Confidence;
        }

        // If confidence is also equal, compare by length (longer wins)
        return a.Length > b.Length;
    }
}
