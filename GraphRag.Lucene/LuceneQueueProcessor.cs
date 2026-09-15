using GraphRag.Lucene.Data;
using GraphRag.Lucene.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Lucene;

/// <summary>
/// Queue processor for indexing documents in Lucene. 
/// It retrieves pending documents from the repository, processes them in batches, 
/// and updates the search index accordingly.
/// </summary>
public sealed class LuceneQueueProcessor
{
    private readonly IChunkRepository _repository;
    private readonly IChunkSearchIndex _index;

    public LuceneQueueProcessor(
        IChunkRepository repository,
        IChunkSearchIndex index)
    {
        _repository = repository;
        _index = index;
    }

    /// <summary>
    /// Processes pending documents in the queue, indexing them in batches.
    /// </summary>
    /// <param name="batchSize">The maximum number of documents to process in a single batch.</param>
    /// <returns>The number of documents processed.</returns>
    public int ProcessPending(int batchSize = 100)
    {
        var documentIds =
            _repository.GetQueuedDocumentIds(batchSize);

        var processed = 0;

        foreach (var documentId in documentIds)
        {
            var chunks =
                _repository.GetDocumentChunks(documentId);

            _index.ReplaceDocument(
                documentId,
                chunks);

            _repository.MarkIndexed(documentId);
            processed++;
        }

        return processed;
    }
}
