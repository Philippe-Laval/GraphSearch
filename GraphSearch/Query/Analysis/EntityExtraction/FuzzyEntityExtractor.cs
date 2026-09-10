using System.Text.RegularExpressions;

namespace GraphSearch.Library.Query.Analysis.EntityExtraction;

/// <summary>
/// Typo-tolerant extractor. Slides a window of 1..N tokens across the query
/// and reports fuzzy matches against a gazetteer using normalized Levenshtein
/// distance.
///
/// <para>
/// Rules of thumb:
/// </para>
/// <list type="bullet">
///   <item>Combine with a strict extractor (<see cref="DictionaryEntityExtractor"/>
///         or <see cref="AhoCorasickGazetteerExtractor"/>) via
///         <see cref="HybridEntityExtractor"/>. Exact matches always outrank
///         fuzzy ones because they carry a higher priority tier and a lower
///         confidence penalty.</item>
///   <item>Enforce a minimum token length (<paramref name="minTokenLength"/>);
///         short strings are noise magnets ("cat" fuzzy-matches almost anything).</item>
///   <item>Confidence is <c>1 - normalizedDistance</c> so a 1-char edit on a
///         9-char entity yields ~0.89.</item>
/// </list>
/// </summary>
public sealed class FuzzyEntityExtractor : IEntityExtractor
{
    // Unicode letters and digits — same tokenizer flavour as elsewhere.
    private static readonly Regex TokenRegex = new(
        @"[\p{L}\p{N}]+",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private readonly IReadOnlyList<EntityDefinition> _entities;
    private readonly int _maxWindowTokens;
    private readonly double _maxNormalizedDistance;
    private readonly int _minTokenLength;

    public FuzzyEntityExtractor(
        IEnumerable<EntityDefinition> entities,
        double maxNormalizedDistance = 0.20,
        int minTokenLength = 4,
        int maxWindowTokens = 4)
    {
        ArgumentNullException.ThrowIfNull(entities);
        if (maxNormalizedDistance is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxNormalizedDistance), "Must be in [0, 1].");
        }
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxWindowTokens);
        ArgumentOutOfRangeException.ThrowIfNegative(minTokenLength);

        _entities = entities.ToList();
        _maxNormalizedDistance = maxNormalizedDistance;
        _minTokenLength = minTokenLength;
        _maxWindowTokens = maxWindowTokens;
    }

    public Task<IReadOnlyList<ExtractedEntity>> ExtractAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);
        cancellationToken.ThrowIfCancellationRequested();

        var tokens = TokenRegex.Matches(query);
        if (tokens.Count == 0 || _entities.Count == 0)
        {
            return Task.FromResult<IReadOnlyList<ExtractedEntity>>(Array.Empty<ExtractedEntity>());
        }

        var accumulator = new List<(ExtractedEntity Entity, int Priority)>();

        for (var i = 0; i < tokens.Count; i++)
        {
            var maxSpan = Math.Min(_maxWindowTokens, tokens.Count - i);
            for (var span = 1; span <= maxSpan; span++)
            {
                var first = tokens[i];
                var last = tokens[i + span - 1];
                var start = first.Index;
                var length = last.Index + last.Length - start;
                var candidate = query.Substring(start, length);
                if (candidate.Length < _minTokenLength)
                {
                    continue;
                }

                var best = FindBest(candidate);
                if (best is null)
                {
                    continue;
                }

                var (def, distance) = best.Value;
                var normalized = (double)distance / Math.Max(candidate.Length, def.Name.Length);
                if (normalized > _maxNormalizedDistance)
                {
                    continue;
                }

                // Skip exact case-insensitive matches — leave those to the dictionary tier.
                if (distance == 0)
                {
                    continue;
                }

                var entity = new ExtractedEntity(
                    Text: candidate,
                    Type: def.Type,
                    Start: start,
                    Length: length,
                    Confidence: Math.Round(1.0 - normalized, 3));

                EntitySpanMerger.AddOrReplace(accumulator, entity, priority: 0);
            }
        }

        return Task.FromResult(EntitySpanMerger.Finalize(accumulator));
    }

    private (EntityDefinition Definition, int Distance)? FindBest(string candidate)
    {
        EntityDefinition? bestDef = null;
        var bestDistance = int.MaxValue;

        foreach (var def in _entities)
        {
            var maxAllowed = (int)Math.Ceiling(
                Math.Max(candidate.Length, def.Name.Length) * _maxNormalizedDistance);
            var distance = BoundedLevenshtein(
                candidate,
                def.Name,
                Math.Max(maxAllowed, 1));

            if (distance >= 0 && distance < bestDistance)
            {
                bestDistance = distance;
                bestDef = def;
                if (distance == 0)
                {
                    break;
                }
            }
        }

        return bestDef is null ? null : (bestDef, bestDistance);
    }

    /// <summary>
    /// Case-insensitive Levenshtein with early exit when the running minimum
    /// exceeds <paramref name="maxDistance"/>. Returns -1 in that case.
    /// </summary>
    private static int BoundedLevenshtein(string a, string b, int maxDistance)
    {
        // Guard on absolute length delta.
        var lenDelta = Math.Abs(a.Length - b.Length);
        if (lenDelta > maxDistance)
        {
            return -1;
        }

        if (a.Length == 0) return b.Length;
        if (b.Length == 0) return a.Length;

        var prev = new int[b.Length + 1];
        var curr = new int[b.Length + 1];
        for (var j = 0; j <= b.Length; j++)
        {
            prev[j] = j;
        }

        for (var i = 1; i <= a.Length; i++)
        {
            curr[0] = i;
            var rowMin = curr[0];
            var ca = char.ToLowerInvariant(a[i - 1]);

            for (var j = 1; j <= b.Length; j++)
            {
                var cb = char.ToLowerInvariant(b[j - 1]);
                var cost = ca == cb ? 0 : 1;
                curr[j] = Math.Min(
                    Math.Min(curr[j - 1] + 1, prev[j] + 1),
                    prev[j - 1] + cost);
                if (curr[j] < rowMin)
                {
                    rowMin = curr[j];
                }
            }

            if (rowMin > maxDistance)
            {
                return -1;
            }

            (prev, curr) = (curr, prev);
        }

        return prev[b.Length];
    }
}
