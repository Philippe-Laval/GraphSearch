using Lucene.Net.Util;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;

namespace GraphSearch.Library.Query.Resolution;

/*
    This is probably the best lexical implementation for your GraphRAG.
   
   I would index:
   
   id
   name
   type
   aliases
   description
   
   Example document:
   
   id          = 1842
   name        = .NET 10
   type        = Technology
   aliases     = DotNet 10
   description = Microsoft .NET 10 runtime
 */


public sealed record GraphEntity(
    long Id,
    string Name,
    string Type,
    IReadOnlyList<string> Aliases,
    string? Description);

/// <summary>
/// Lucene index creation
/// </summary>
public sealed class LuceneEntityIndexWriter
{
    private readonly string _path;

    public LuceneEntityIndexWriter(string path)
    {
        _path = path;
    }

    public void Index(
        IEnumerable<GraphEntity> entities)
    {
        using var directory =
            FSDirectory.Open(_path);

        var analyzer =
            new StandardAnalyzer(
                LuceneVersion.LUCENE_48);

        var config =
            new IndexWriterConfig(
                LuceneVersion.LUCENE_48,
                analyzer);

        using var writer =
            new IndexWriter(
                directory,
                config);

        foreach (var entity in entities)
        {
            var document = new Document
            {
                new StringField(
                    "id",
                    entity.Id.ToString(),
                    Field.Store.YES),

                new TextField(
                    "name",
                    entity.Name,
                    Field.Store.YES),

                new StringField(
                    "type",
                    entity.Type,
                    Field.Store.YES),

                new TextField(
                    "aliases",
                    string.Join(
                        " ",
                        entity.Aliases),
                    Field.Store.YES),

                new TextField(
                    "description",
                    entity.Description ?? "",
                    Field.Store.NO)
            };

            writer.UpdateDocument(
                new Term(
                    "id",
                    entity.Id.ToString()),
                document);
        }

        writer.Commit();
    }
}