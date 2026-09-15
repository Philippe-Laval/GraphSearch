using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Lucene.Models;

/// <summary>
/// ChunkSearchHit represents a single search result from a chunk search, 
/// containing the chunk ID, document ID, and the relevance score of the hit.
/// </summary>
/// <param name="ChunkId">The unique identifier of the chunk.</param>
/// <param name="DocumentId">The unique identifier of the document containing the chunk.</param>
/// <param name="Score">The relevance score of the search hit.</param>
public sealed record ChunkSearchHit(
  int ChunkId,
  int DocumentId,
  float Score);
