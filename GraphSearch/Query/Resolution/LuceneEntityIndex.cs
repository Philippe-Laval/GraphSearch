using Lucene.Net.Util;

namespace GraphSearch.Query.Resolution;

using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;

/*
One caveat: because StandardAnalyzer tokenizes fields, exact matching of things like .NET 10 needs some care. In production I'd use two fields:
   
   name
   name_exact
   
   where:
   name       → TextField
   name_exact → StringField
   
   Then:
   .NET 10
   
   can be matched exactly while still allowing normal lexical search.
 */

/*
Add fuzzy matching
   
   This is particularly useful for entity resolution.
   
   For example:
   
   Microsoft
   Microsft
   Microsof
   
   Lucene.NET has FuzzyQuery, based on edit distance.
   
   We can add:
   
   boolean.Add(
       new FuzzyQuery(
           new Term(
               "name",
               text.ToLowerInvariant()),
           maxEdits: 2),
       Occur.SHOULD);
   
   However, don't use fuzzy matching indiscriminately.
   
   For short names, versions, acronyms, etc., it can produce bad candidates. The Lucene documentation itself notes limitations around very short terms.
   
   I'd use it only as a fallback:
   
   1. Exact
   2. Alias
   3. BM25
   4. Fuzzy
 */


/// <summary>
/// Lucene search
/// </summary>
public sealed class LuceneEntityIndex : IEntityIndex, IDisposable
{
    private readonly DirectoryReader _reader;
    private readonly IndexSearcher _searcher;
    private readonly QueryParser _parser;

    public LuceneEntityIndex(string path)
    {
        var directory =
            FSDirectory.Open(path);

        _reader =
            DirectoryReader.Open(directory);

        _searcher =
            new IndexSearcher(_reader);

        var analyzer =
            new StandardAnalyzer(
                LuceneVersion.LUCENE_48);

        _parser =
            new QueryParser(
                LuceneVersion.LUCENE_48,
                "name",
                analyzer);
    }

    public Task<IReadOnlyList<EntityCandidate>> SearchAsync(
        string text,
        string? entityType,
        int topK,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        if (topK <= 0)
            return Task.FromResult<
                IReadOnlyList<EntityCandidate>>([]);

        var query =
            BuildQuery(text, entityType);

        var hits =
            _searcher.Search(
                query,
                topK);

        var results =
            new List<EntityCandidate>();

        foreach (var hit in hits.ScoreDocs)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var document =
                _searcher.Doc(hit.Doc);

            var id =
                long.Parse(
                    document.Get("id"));

            results.Add(
                new EntityCandidate(
                    id,
                    document.Get("name"),
                    document.Get("type"),
                    NormalizeScore(
                        hit.Score)));
        }

        return Task.FromResult<
            IReadOnlyList<EntityCandidate>>(results);
    }

    private Query BuildQuery(
        string text,
        string? entityType)
    {
        var escaped = QueryParser.Escape(text);

        var boolean = new BooleanQuery();

        // Exact name match.
        boolean.Add(
            new TermQuery(
                new Term(
                    "name",
                    text.ToLowerInvariant())),
            Occur.SHOULD);

        // Normal lexical query.
        var parsed = _parser.Parse(escaped);

        boolean.Add(
            parsed,
            Occur.SHOULD);

        // Alias search.
        boolean.Add(
            new TermQuery(
                new Term(
                    "aliases",
                    text.ToLowerInvariant())),
            Occur.SHOULD);

        // Optional entity type.
        if (!string.IsNullOrWhiteSpace(entityType))
        {
            boolean.Add(
                new TermQuery(
                    new Term(
                        "type",
                        entityType)),
                Occur.MUST);
        }

        return boolean;
    }

    private static double NormalizeScore(float score)
    {
        // Converts Lucene's unbounded-ish score
        // into approximately 0..1.

        return score / (1.0 + score);
    }

    public void Dispose()
    {
        _reader.Dispose();
    }
}