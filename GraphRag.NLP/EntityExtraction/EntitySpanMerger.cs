using GraphRag.NLP.Models;

namespace GraphRag.NLP.EntityExtraction
{
    /// <summary>
    /// Shared helpers to merge overlapping <see cref="ExtractedEntity"/> spans while preserving
    /// the highest-priority and highest-confidence match.
    /// </summary>
    public static class EntitySpanMerger
    {
        public static void AddOrReplace(
            List<(ExtractedEntity Entity, int Priority)> accumulator,
            ExtractedEntity candidate,
            int priority)
        {
            for (var i = accumulator.Count - 1; i >= 0; i--)
            {
                var (existing, existingPriority) = accumulator[i];
                if (!Overlap(candidate, existing))
                {
                    continue;
                }

                if (Wins(candidate, priority, existing, existingPriority))
                {
                    accumulator.RemoveAt(i);
                }
                else
                {
                    return;
                }
            }

            accumulator.Add((candidate, priority));
        }

        public static IReadOnlyList<ExtractedEntity> Finalize(
            List<(ExtractedEntity Entity, int Priority)> accumulator)
        {
            return accumulator
                .Select(x => x.Entity)
                .OrderBy(e => e.Start)
                .ThenByDescending(e => e.Length)
                .ToList();
        }

        private static bool Overlap(ExtractedEntity a, ExtractedEntity b)
        {
            var aEnd = a.Start + a.Length;
            var bEnd = b.Start + b.Length;
            return a.Start < bEnd && aEnd > b.Start;
        }

        private static bool Wins(
            ExtractedEntity a,
            int aPriority,
            ExtractedEntity b,
            int bPriority)
        {
            if (aPriority != bPriority)
            {
                return aPriority > bPriority;
            }

            if (Math.Abs(a.Confidence - b.Confidence) > 1e-9)
            {
                return a.Confidence > b.Confidence;
            }

            return a.Length > b.Length;
        }
    }
}
