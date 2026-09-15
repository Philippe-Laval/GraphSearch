using GraphRag.Core.Models;
using GraphRag.Lucene;
using GraphRag.Lucene.Models;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using Document = Lucene.Net.Documents.Document;

namespace GraphRag.BM25;

/// <summary>
/// Creates and manages a Lucene index for storing and retrieving chunk documents. 
/// Provides methods for indexing, updating, and deleting documents in the index.
/// </summary>
public sealed class LuceneIndex
{
    public IndexWriter Writer { get; private set; }

    public LuceneIndex(string folder)
    {
        var directory = FSDirectory.Open(folder);

        var analyzer =
            new StandardAnalyzer(LuceneVersion.LUCENE_48);

        var config =
            new IndexWriterConfig(
                LuceneVersion.LUCENE_48,
                analyzer);

        Writer = new IndexWriter(directory, config);
    }

    /// <summary>
    /// Indexes a collection of chunk documents into the Lucene index.
    /// </summary>
    /// <param name="chunks">The chunk documents to index.</param>
    /// <returns>A task that represents the asynchronous indexing operation.</returns>
    public async Task IndexAsync(IEnumerable<ChunkDocument> chunks)
    {
        foreach (var chunk in chunks)
        {
            var document = new Document
            {
                new StringField(
                    LuceneFields.ChunkId,
                    chunk.Id.ToString(),
                    Field.Store.YES),

                new StringField(
                    LuceneFields.DocumentId,
                    chunk.DocumentId.ToString(),
                    Field.Store.YES),

                new TextField(
                    LuceneFields.Text,
                    chunk.Text,
                    Field.Store.YES),

                //new StringField(
                //    LuceneFields.EntityId,
                //    string.Join(",", chunk.EntityIds),
                //    Field.Store.YES)
            };

            foreach (var entityId in chunk.EntityIds
                     .Where(static value =>
                         !string.IsNullOrWhiteSpace(value))
                     .Distinct(StringComparer.Ordinal))
            {
                document.Add(
                    new StringField(
                        LuceneFields.EntityId,
                        entityId,
                        Field.Store.NO));
            }

            if (chunk.Metadata is not null)
            {
                foreach (var (key, value) in chunk.Metadata)
                {
                    document.Add(
                        new StringField(
                            LuceneFields.MetadataPrefix + NormalizeMetadataKey(key),
                            value,
                            Field.Store.NO));
                }
            }

            Writer.AddDocument(document);
        }

        Writer.Commit();

        await Task.CompletedTask;
    }

    /// <summary>
    /// Normalizes the key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    private static string NormalizeMetadataKey(
        string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var normalized = new string(
            key.ToLowerInvariant()
                .Select(character =>
                    char.IsAsciiLetterOrDigit(character)
                        ? character
                        : '_')
                .ToArray());

        return normalized;
    }
    
    /// <summary>
    /// Updates an existing chunk document in the Lucene index.
    /// </summary>
    /// <param name="chunk">The chunk document to update.</param>
    public void UpdateDocument(ChunkDocument chunk)
    {
        var document = new Document
            {
                new StringField(
                    "ChunkId",
                    chunk.Id.ToString(),
                    Field.Store.YES),

                new StringField(
                    "DocumentId",
                    chunk.DocumentId.ToString(),
                    Field.Store.YES),

                new TextField(
                    "Content",
                    chunk.Text,
                    Field.Store.YES),

                new StringField(
                    "Entities",
                    string.Join(",", chunk.EntityIds),
                    Field.Store.YES)
            };

        Writer.UpdateDocument(
            new Term("ChunkId", chunk.Id.ToString()),
            document);

        Writer.Commit();
    }

    /// <summary>
    /// Deletes a chunk document from the Lucene index based on its ChunkId.
    /// </summary>
    /// <param name="chunk">The chunk document to delete.</param>
    public void DeleteDocument(ChunkDocument chunk)
    {
        Writer.DeleteDocuments(new Term("ChunkId", chunk.Id.ToString()));
        Writer.Commit();
    }
}
