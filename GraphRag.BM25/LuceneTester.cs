using GraphRag.Core.Models;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Util;
using System;
using System.Collections.Generic;
using System.Text;
using static Lucene.Net.Util.Packed.PackedInt32s;
using static System.Net.WebRequestMethods;

namespace GraphRag.BM25;

internal class LuceneTester
{
    public BooleanQuery CreateTestQuery()
    {
        var builder = new BooleanQuery();

        builder.Add(
            new TermQuery(new Term("Content", "louvain")),
            Occur.SHOULD);

        builder.Add(
            new TermQuery(new Term("Content", "community")),
            Occur.SHOULD);

        builder.Add(
            new PhraseQuery
            {
                new Term("Content", "community"),
                new Term("Content", "detection")
            },
            Occur.SHOULD);

        var query = builder;
        return query;
    }

    public BooleanQuery CreateTestQueryWithEntities()
    {
        var builder = new BooleanQuery();
        builder.Add(
            new TermQuery(new Term("Content", "louvain")),
            Occur.SHOULD);
        builder.Add(
            new TermQuery(new Term("Content", "community")),
            Occur.SHOULD);
        builder.Add(
            new PhraseQuery
            {
                new Term("Content", "community"),
                new Term("Content", "detection")
            },
            Occur.SHOULD);
        builder.Add(
            new TermQuery(new Term("Entities", "entity1")),
            Occur.SHOULD);
        builder.Add(
            new TermQuery(new Term("Entities", "entity2")),
            Occur.SHOULD);
        var query = builder;
        return query;
    }

    //public BooleanQuery CreateTestQueryWithEntities()
    //{
    //    var query = new BooleanQuery
    //    {
    //        {
    //            new TermQuery(new Term("DocumentId", "paper123")),
    //            Occur.FILTER
    //        },
    //        {
    //            new QueryParser(
    //                LuceneVersion.LUCENE_48,
    //                "Content",
    //                analyzer)
    //                .Parse("Leiden"),
    //            Occur.MUST
    //        }
    //    };

    //    //FILTER clauses don't affect the BM25 score—they simply restrict the candidate set.
    //}
}
