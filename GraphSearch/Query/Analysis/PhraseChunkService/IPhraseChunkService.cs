namespace GraphSearch.Library.Query.Analysis.PhraseChunkService;

/// <summary>
/// Emits candidate phrase spans from a query. Typical implementations wrap
/// spaCy noun-chunks, a chunker parser, or a lightweight regex heuristic.
///
/// <para>
/// Used by <see cref="PhraseChunkEntityExtractor"/> and (optionally) by
/// <see cref="EmbeddingCandidateEntityExtractor"/> as its candidate source.
/// </para>
/// </summary>
public interface IPhraseChunkService
{
    /// <summary>
    /// Extrait de manière asynchrone les segments de phrase du texte fourni.
    /// </summary>
    /// <param name="text">Texte à analyser pour en extraire des segments de phrase.</param>
    /// <param name="cancellationToken">Jeton permettant d’observer les demandes d’annulation.</param>
    /// <returns>Tâche qui représente l’opération asynchrone et dont le résultat contient une liste en lecture seule des segments
    /// de phrase extraits.</returns>
    Task<IReadOnlyList<PhraseChunk>> ExtractChunksAsync(
        string text,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// A single candidate span produced by an <see cref="IPhraseChunkService"/>.
/// </summary>
public sealed record PhraseChunk(string Text, int Start, int Length);
