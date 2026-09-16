using GraphRag.NLP.EntityExtraction;
using GraphRag.NLP.Models;
using System;

namespace GraphRag.NLP.Tests.EntityExtraction
{
    [TestClass]
    public sealed class AhoCorasickGazetteerExtractorTests
    {
        [TestMethod]
        public async Task ExtractAsync_FindsCaseInsensitiveMatchesAndPreservesSourceCasing()
        {
            var extractor = new AhoCorasickGazetteerExtractor(
            [
                new EntityDefinition("Microsoft", "Organization"),
                new EntityDefinition("Azure SQL", "Database"),
            ]);
            const string text = "microsoft integrates AZURE sql.";

            IReadOnlyList<ExtractedEntity> result = await extractor.ExtractAsync(text);

            Assert.HasCount(2, result);
            AssertEntity(result, "microsoft", "Organization", 1.0, text.IndexOf("microsoft", StringComparison.Ordinal));
            AssertEntity(result, "AZURE sql", "Database", 1.0, text.IndexOf("AZURE sql", StringComparison.Ordinal));
        }

        [TestMethod]
        public async Task ExtractAsync_PrefersLongestOverlappingGazetteerMatch()
        {
            var extractor = new AhoCorasickGazetteerExtractor(
            [
                new EntityDefinition("Azure", "Cloud"),
                new EntityDefinition("Azure SQL", "Database"),
            ]);
            const string text = "Azure SQL supports analytics.";

            IReadOnlyList<ExtractedEntity> result = await extractor.ExtractAsync(text);

            Assert.HasCount(1, result);

            var entity = result[0];
            Assert.AreEqual("Azure SQL", entity.Text);
            Assert.AreEqual("Database", entity.Type);
            Assert.AreEqual(0, entity.Start);
            Assert.AreEqual("Azure SQL".Length, entity.Length);
            Assert.AreEqual(1.0, entity.Confidence, 0.001);
        }

        [TestMethod]
        public async Task ExtractAsync_UsesConfiguredConfidence()
        {
            var extractor = new AhoCorasickGazetteerExtractor(
                [new EntityDefinition("GraphSearch", "Product")],
                confidence: 0.72);
            const string text = "GraphSearch indexes documents.";

            IReadOnlyList<ExtractedEntity> result = await extractor.ExtractAsync(text);

            Assert.HasCount(1, result);

            var entity = result[0];
            Assert.AreEqual("GraphSearch", entity.Text);
            Assert.AreEqual("Product", entity.Type);
            Assert.AreEqual(0, entity.Start);
            Assert.AreEqual("GraphSearch".Length, entity.Length);
            Assert.AreEqual(0.72, entity.Confidence, 0.001);
        }

        private static void AssertEntity(
            IReadOnlyList<ExtractedEntity> entities,
            string expectedText,
            string expectedType,
            double expectedConfidence,
            int expectedStart)
        {
            var entity = entities.FirstOrDefault(e => e.Text == expectedText);
            Assert.IsNotNull(entity);
            Assert.AreEqual(expectedType, entity.Type);
            Assert.AreEqual(expectedStart, entity.Start);
            Assert.AreEqual(expectedText.Length, entity.Length);
            Assert.AreEqual(expectedConfidence, entity.Confidence, 0.001);
        }
    }
}
