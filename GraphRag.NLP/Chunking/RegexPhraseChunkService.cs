using System.Text.RegularExpressions;

namespace GraphRag.NLP.Chunking
{
    /// <summary>
    /// Cheap, dependency-free fallback <see cref="IPhraseChunkService"/>.
    /// Emits runs of capitalized words as phrase candidates.
    /// Extracts capitalized phrases with connectors : "Research and Development", "University of Washington", "City of London".
    /// Returns empty list for lowercase-only text.
    /// Preserves apostrophes, hyphens, and embedded digits in chunk text : "O'Reilly Media", "GPT-4", "Windows11".
    /// </summary>
    public sealed class RegexPhraseChunkService : IPhraseChunkService
    {
        private static readonly Regex ChunkRegex = new(
            @"\p{Lu}[\p{Ll}\p{Lu}\p{Nd}'’\-]*(?:\s+(?:of|de|du|des|la|le|les|the|and|et|von|van|di|da|of the)\s+\p{Lu}[\p{Ll}\p{Lu}\p{Nd}'’\-]*)*(?:\s+\p{Lu}[\p{Ll}\p{Lu}\p{Nd}'’\-]*)*",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// 
        /// Extracts capitalized phrases with connectors : "Research and Development", "University of Washington", "City of London".
        /// Returns empty list for lowercase-only text.
        /// Preserves apostrophes, hyphens, and embedded digits in chunk text : "O'Reilly Media", "GPT-4", "Windows11".
        /// </summary>
        /// <param name="text"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IReadOnlyList<PhraseChunk>> ExtractChunksAsync(
            string text,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(text);
            cancellationToken.ThrowIfCancellationRequested();

            var chunks = new List<PhraseChunk>();
            foreach (Match match in ChunkRegex.Matches(text))
            {
                if (match.Length == 0)
                {
                    continue;
                }

                chunks.Add(new PhraseChunk(match.Value, match.Index, match.Length));
            }

            return Task.FromResult<IReadOnlyList<PhraseChunk>>(chunks);
        }
    }
}
