using System.Text.RegularExpressions;

namespace GraphSearch.Library.Query.Analysis.PhraseChunkService;

/// <summary>
/// Cheap, dependency-free fallback <see cref="IPhraseChunkService"/>. Emits
/// runs of capitalized words (optionally joined by lowercase function words
/// such as <c>of</c>, <c>de</c>, <c>the</c>, <c>le</c>) as phrase candidates.
///
/// <para>
/// Not a replacement for a real chunker — it's designed to give
/// <see cref="PhraseChunkEntityExtractor"/> and
/// <see cref="EmbeddingCandidateEntityExtractor"/> something to work with
/// when the spaCy microservice is unavailable. Language-agnostic and works
/// on Unicode letter categories.
/// </para>
/// </summary>
public sealed class RegexPhraseChunkService : IPhraseChunkService
{
    private static readonly Regex ChunkRegex = new(
        // Sequence of Capitalized-Words possibly joined by short lowercase glue.
        @"\p{Lu}[\p{Ll}\p{Lu}\p{Nd}'’\-]*(?:\s+(?:of|de|du|des|la|le|les|the|and|et|von|van|di|da|of the)\s+\p{Lu}[\p{Ll}\p{Lu}\p{Nd}'’\-]*)*(?:\s+\p{Lu}[\p{Ll}\p{Lu}\p{Nd}'’\-]*)*",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public Task<IReadOnlyList<PhraseChunk>> ExtractChunksAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        cancellationToken.ThrowIfCancellationRequested();

        var chunks = new List<PhraseChunk>();
        foreach (Match m in ChunkRegex.Matches(text))
        {
            if (m.Length == 0)
            {
                continue;
            }
            chunks.Add(new PhraseChunk(m.Value, m.Index, m.Length));
        }
        return Task.FromResult<IReadOnlyList<PhraseChunk>>(chunks);
    }
}
