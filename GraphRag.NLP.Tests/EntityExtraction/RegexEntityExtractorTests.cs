using GraphRag.NLP.EntityExtraction;
using GraphRag.NLP.Models;
using System;

namespace GraphRag.NLP.Tests.EntityExtraction
{
    [TestClass]
    public sealed class RegexEntityExtractorTests
    {
        [TestMethod]
        public async Task ExtractAsync_CreateDefault_ExtractsStructuredEntities()
        {
            var extractor = RegexEntityExtractor.CreateDefault();
            const string text = "Release 1.2.3-beta shipped on 2026-09-16. Visit https://example.com/path contact support@example.com";

            IReadOnlyList<ExtractedEntity> result = await extractor.ExtractAsync(text);

            Assert.HasCount(4, result);

            AssertEntity(result, "1.2.3-beta", "Version", 0.85);
            AssertEntity(result, "2026-09-16", "Date", 0.95);
            AssertEntity(result, "https://example.com/path", "Url", 0.99);
            AssertEntity(result, "support@example.com", "Email", 0.99);
        }

        [TestMethod]
        public async Task ExtractAsync_UsesConfiguredCaptureGroup()
        {
            // Example: Extract the build number from a string like "Current build #42 is stable."
            // Uses a named capture group "number" to extract just the digits after the '#'.
            var extractor = RegexEntityExtractor.CreateExtractor(
                new RegexEntityRule("BuildNumber", @"build\s+#(?<number>\d+)", 0.77, "number"));
            const string text = "Current build #42 is stable.";

            IReadOnlyList<ExtractedEntity> result = await extractor.ExtractAsync(text);

            Assert.HasCount(1, result);

            var entity = result[0];
            Assert.AreEqual("42", entity.Text);
            Assert.AreEqual("BuildNumber", entity.Type);
            Assert.AreEqual(text.IndexOf("42", StringComparison.Ordinal), entity.Start);
            Assert.AreEqual(2, entity.Length);
            Assert.AreEqual(0.77, entity.Confidence, 0.001);
        }

        [TestMethod]
        public async Task ExtractAsync_PrefersHigherConfidenceForOverlappingMatches()
        {
            // Example: Two overlapping regex rules, one for a general SKU pattern and another for a specific SKU.
            // The extractor should prefer the match with the higher confidence score.
            var extractor = RegexEntityExtractor.CreateExtractor(
                new RegexEntityRule("Sku", @"\bABC-\d{3}\b", 0.70),
                new RegexEntityRule("SpecialSku", @"\bABC-123\b", 0.95));
            const string text = "Code ABC-123 is active.";

            IReadOnlyList<ExtractedEntity> result = await extractor.ExtractAsync(text);

            Assert.HasCount(1, result);

            var entity = result[0];
            Assert.AreEqual("ABC-123", entity.Text);
            Assert.AreEqual("SpecialSku", entity.Type);
            Assert.AreEqual(text.IndexOf("ABC-123", StringComparison.Ordinal), entity.Start);
            Assert.AreEqual("ABC-123".Length, entity.Length);
            Assert.AreEqual(0.95, entity.Confidence, 0.001);
        }

        private static void AssertEntity(
            IReadOnlyList<ExtractedEntity> entities,
            string expectedText,
            string expectedType,
            double expectedConfidence)
        {
            var entity = entities.FirstOrDefault(e => e.Text == expectedText);
            Assert.IsNotNull(entity);
            Assert.AreEqual(expectedType, entity.Type);
            Assert.AreEqual(expectedText.Length, entity.Length);
            Assert.AreEqual(expectedConfidence, entity.Confidence, 0.001);
        }
    }
}
