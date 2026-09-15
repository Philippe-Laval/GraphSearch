using GraphRag.Core.Models;
using GraphRag.BM25.Retrievers;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.BM25.Tests.Retrievers;

[TestClass]
public sealed class HybridRetrieverTests
{
    [TestMethod]
    public async Task TestRetrieveAsync()
    {
        HybridRetriever retriever = new HybridRetriever(@"C:\LuceneIndex");

        IReadOnlyList<SearchResult> result = await retriever.RetrieveAsync("DDM Qualiparc", 10);

        Assert.IsNotNull(result);
        Assert.IsNotEmpty(result);
    }
}