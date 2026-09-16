using System.Text.RegularExpressions;
using GraphRag.NLP.Models;

namespace GraphRag.NLP.EntityExtraction
{
    /// <summary>
    /// Typo-tolerant extractor that reports fuzzy matches against a gazetteer.
    /// Skips exact matches and only returns matches with a normalized Levenshtein distance below a specified threshold.
    /// Prefers best overlapping fuzzy match (text with maximum length similarity).
    /// </summary>
    public sealed class FuzzyEntityExtractor : IEntityExtractor
    {
        private static readonly Regex TokenRegex = new(
            @"[\p{L}\p{N}]+",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private readonly IReadOnlyList<EntityDefinition> _entities;
        private readonly int _maxWindowTokens;
        private readonly double _maxNormalizedDistance;
        private readonly int _minTokenLength;

        public static FuzzyEntityExtractor CreateExtractor(params EntityDefinition[] entities)
            => new FuzzyEntityExtractor(entities);

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
            string text,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(text);
            cancellationToken.ThrowIfCancellationRequested();

            var tokens = TokenRegex.Matches(text);
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
                    var candidate = text.Substring(start, length);
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
                    if (normalized > _maxNormalizedDistance || distance == 0)
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

        /// <summary>
        /// Recherche la définition d’entité la plus proche du texte fourni 
        /// en minimisant la distance de Levenshtein bornée.
        /// </summary>
        /// <remarks>La distance maximale autorisée par entité est calculée à partir de la longueur la
        /// plus grande entre l’entrée et le nom d’entité, puis multipliée par un seuil de distance
        /// normalisée.</remarks>
        /// <param name="candidate">Texte à comparer aux noms d’entités.</param>
        /// <returns>Un tuple contenant la définition correspondante et sa distance minimale lorsque qu’une correspondance valide
        /// est trouvée ; sinon, <see langword="null"/>.</returns>
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
        /// Calcule la distance de Levenshtein entre deux chaînes sans tenir compte de la casse, avec une limite
        /// maximale.
        /// </summary>
        /// <remarks>Retourne également <c>-1</c> si l'écart de longueur initial entre les deux chaînes
        /// dépasse <paramref name="maxDistance" />.</remarks>
        /// <param name="a">Première chaîne à comparer.</param>
        /// <param name="b">Deuxième chaîne à comparer.</param>
        /// <param name="maxDistance">Distance maximale acceptée avant arrêt anticipé du calcul.</param>
        /// <returns>Distance de Levenshtein calculée si elle est inférieure ou égale à <paramref name="maxDistance" /> ; sinon
        /// <c>-1</c>.</returns>
        private static int BoundedLevenshtein(string a, string b, int maxDistance)
        {
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
}
