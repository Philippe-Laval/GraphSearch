using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Lucene.Models;

/// <summary>
/// PersistedDocument represents a document that has been 
/// ingested and persisted in the system, along with its associated chunks.
/// </summary>
/// <param name="Document">The source document that has been persisted.</param>
/// <param name="Chunks">The list of chunks associated with the persisted document.</param>
public sealed record PersistedDocument(
    SourceDocument Document,
    IReadOnlyList<ChunkDocument> Chunks);