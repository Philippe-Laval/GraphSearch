using GraphRag.NLP.EntityExtraction;
using GraphRag.NLP.Models;
using System;

namespace GraphRag.NLP.Tests.EntityExtraction
{
    [TestClass]
    public sealed class FuzzyEntityExtractorTests
    {
        [TestMethod]
        public async Task ExtractAsync_ReturnsTypoMatchWithSpanAndConfidence()
        {
            var extractor = FuzzyEntityExtractor.CreateExtractor(new EntityDefinition("Microsoft", "Organization"));
            const string text = "Micosoft builds tools.";

            IReadOnlyList<ExtractedEntity> result = await extractor.ExtractAsync(text);

            Assert.HasCount(1, result);

            var entity = result[0];
            Assert.AreEqual("Micosoft", entity.Text);
            Assert.AreEqual("Organization", entity.Type);
            Assert.AreEqual(text.IndexOf("Micosoft", StringComparison.Ordinal), entity.Start);
            Assert.AreEqual("Micosoft".Length, entity.Length);
            Assert.AreEqual(0.889, entity.Confidence, 0.001);
        }

        [TestMethod]
        public async Task ExtractAsync_SkipsExactMatches()
        {
            var extractor = FuzzyEntityExtractor.CreateExtractor(new EntityDefinition("Microsoft", "Organization"));

            IReadOnlyList<ExtractedEntity> result = await extractor.ExtractAsync("Microsoft ships updates.");

            Assert.HasCount(0, result);
        }

        [TestMethod]
        public async Task ExtractAsync_PrefersBestOverlappingFuzzyMatch()
        {
            var extractor = FuzzyEntityExtractor.CreateExtractor(
                new EntityDefinition("Azure", "Cloud"),
                new EntityDefinition("Azure SQL", "Database"));
            const string text = "We use Azur SQL in production.";

            IReadOnlyList<ExtractedEntity> result = await extractor.ExtractAsync(text);

            Assert.HasCount(1, result);

            var entity = result[0];
            Assert.AreEqual("Azur SQL", entity.Text);
            Assert.AreEqual("Database", entity.Type);
            Assert.AreEqual(text.IndexOf("Azur SQL", StringComparison.Ordinal), entity.Start);
            Assert.AreEqual("Azur SQL".Length, entity.Length);
            Assert.AreEqual(0.889, entity.Confidence, 0.001);
        }

    }
}
