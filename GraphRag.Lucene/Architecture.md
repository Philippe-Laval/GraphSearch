# Recommended architecture

Use SQLite as the source of truth and Lucene.NET as a rebuildable search index:

- Markdown files remain the original source.
- SQLite stores documents, paths, file hashes, chunks, chunk text, entities, and metadata.
- Lucene stores searchable fields and identifiers.
- Search Lucene for matching chunk IDs, then load the complete chunks from SQLite.

I recommend storing chunk text in SQLite even though it duplicates information from the Markdown files. It gives you:

- Stable retrieval after a source file changes.
- Straightforward Lucene index rebuilding.
- Transactional document/chunk replacement.
- Easier metadata updates.
- A clear distinction between persistent data and the derived search index.

Do not depend on Lucene as your only database. Lucene documents are optimized for indexing and search, rather than relational updates and authoritative storage.

Lucene.NET 4.8 remains a prerelease, with 4.8.0-beta00018 currently documented. Its packages target .NET 8, .NET 6, and .NET Standard, so they can be consumed by a net10.0 application.



# About storing Text in Lucene

The example uses:

new TextField("text", chunk.Text, Field.Store.NO)

This means:

- The text is searchable.
- The original text cannot be returned from Lucene.
- The search result contains chunk_id.
- SQLite supplies the actual chunk.

Use Field.Store.YES only when:

- You want Lucene to work without SQLite.
- You need simple result snippets directly from the index.
- You accept the additional index size and duplicated data.

For sophisticated highlighting, you may need stored text and potentially term vectors/offsets. Term-vector support is optional per field in Lucene.NET

# Important operational practices

1. Treat the Lucene index as disposable. You should be able to delete its directory and rebuild it entirely from SQLite.
2. Replace all chunks for a document together. Chunk boundaries frequently change, so updating individual chunks can leave stale results.
3. Use one IndexWriter per index. It is designed to handle indexing operations; do not open a new writer for every chunk.
4. Commit per document batch, not per chunk.
5. Do not expose raw QueryParser syntax unless intended. The example escapes user input, converting it into a literal text query.
6. Normalize entity identifiers. Decide whether they are case-sensitive and apply the same normalization during indexing and searching.
7. Restrict indexed metadata keys. Arbitrary metadata keys can create an uncontrolled number of Lucene fields.
8. Back up SQLite, not Lucene. Lucene can be rebuilt.
9. Store the source content hash. This avoids unnecessarily rechunking and reindexing unchanged Markdown files.
10. Handle deleted files. After scanning, compare discovered document IDs with the SQLite documents table and remove records no longer present; the deletion should also be propagated to Lucene through a deletion queue or tombstone entry.