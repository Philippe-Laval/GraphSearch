namespace GraphRag.NLP.Chunking
{
    /// <summary>
    /// Emits candidate phrase spans from text. Typical implementations wrap spaCy noun chunks,
    /// a parser-based chunker, or lightweight heuristics.
    /// </summary>
    public interface IPhraseChunkService
    {
        /// <summary>
        /// Asynchronously extracts phrase chunks from the provided text.
        /// </summary>
        /// <param name="text">Text to analyze.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The extracted phrase chunks.</returns>
        Task<IReadOnlyList<PhraseChunk>> ExtractChunksAsync(
            string text,
            CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// A single candidate phrase span produced by an <see cref="IPhraseChunkService"/>.
    /// </summary>
    /// <param name="Text">The chunk text.</param>
    /// <param name="Start">The zero-based start position in the source text.</param>
    /// <param name="Length">The chunk length.</param>
    public sealed record PhraseChunk(string Text, int Start, int Length);
}
