using GraphRag.NLP.Chunking;
using GraphRag.NLP.EntityExtraction;
using GraphRag.NLP.Models;

namespace GraphRag.NLP.Tests.EntityExtraction
{
    [TestClass]
    public sealed class PhraseChunkEntityExtractorTests
    {
        [TestMethod]
        public void Constructor_WithNullChunker_ThrowsArgumentNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new PhraseChunkEntityExtractor(null!));
        }

        [TestMethod]
        public void Constructor_WithBlankDefaultType_ThrowsArgumentException()
        {
            IPhraseChunkService chunker = new RegexPhraseChunkService();

            Assert.ThrowsExactly<ArgumentException>(
                () => new PhraseChunkEntityExtractor(chunker, defaultType: "   "));
        }

        [TestMethod]
        public async Task ExtractAsync_WithBlankText_ThrowsArgumentException()
        {
            IEntityExtractor extractor = new PhraseChunkEntityExtractor(new RegexPhraseChunkService());

            await Assert.ThrowsExactlyAsync<ArgumentException>(
                () => extractor.ExtractAsync("   "));
        }

        [TestMethod]
        public async Task ExtractAsync_WithCanceledToken_PropagatesOperationCanceledException()
        {
            IEntityExtractor extractor = new PhraseChunkEntityExtractor(new RegexPhraseChunkService());
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            await Assert.ThrowsExactlyAsync<OperationCanceledException>(
                () => extractor.ExtractAsync("Microsoft Graph powers search.", cancellationTokenSource.Token));
        }

        [TestMethod]
        public async Task ExtractAsync_MapsRegexPhraseChunksToEntities()
        {
            IEntityExtractor extractor = new PhraseChunkEntityExtractor(new RegexPhraseChunkService());
            const string text = "University of Washington works with O'Reilly Media on Windows11.";

            IReadOnlyList<ExtractedEntity> entities = await extractor.ExtractAsync(text);

            Assert.HasCount(3, entities);
            AssertEntity(entities, text, "University of Washington", "NounPhrase", 0.5);
            AssertEntity(entities, text, "O'Reilly Media", "NounPhrase", 0.5);
            AssertEntity(entities, text, "Windows11", "NounPhrase", 0.5);
        }

        [TestMethod]
        public async Task ExtractAsync_AppliesCustomTypeConfidenceAndMinimumLength()
        {
            IEntityExtractor extractor = new PhraseChunkEntityExtractor(
                new RegexPhraseChunkService(),
                defaultType: "Topic",
                confidence: 0.77,
                minLength: 5);

            const string text = "AI powers Microsoft Graph at Build.";

            IReadOnlyList<ExtractedEntity> entities = await extractor.ExtractAsync(text);

            Assert.HasCount(2, entities);
            Assert.IsFalse(entities.Any(entity => entity.Text == "AI"));
            AssertEntity(entities, text, "Microsoft Graph", "Topic", 0.77);
            AssertEntity(entities, text, "Build", "Topic", 0.77);
        }

        [TestMethod]
        public async Task ExtractAsync_WhenChunkerFindsNothing_ReturnsEmpty()
        {
            IEntityExtractor extractor = new PhraseChunkEntityExtractor(new RegexPhraseChunkService());

            IReadOnlyList<ExtractedEntity> entities = await extractor.ExtractAsync(
                "this sentence contains no capitalized phrase candidates.");

            Assert.IsEmpty(entities);
        }

        private static void AssertEntity(
            IReadOnlyList<ExtractedEntity> entities,
            string sourceText,
            string expectedText,
            string expectedType,
            double expectedConfidence)
        {
            ExtractedEntity? entity = entities.FirstOrDefault(candidate => candidate.Text == expectedText);
            Assert.IsNotNull(entity);
            Assert.AreEqual(expectedType, entity.Type);
            Assert.AreEqual(sourceText.IndexOf(expectedText, StringComparison.Ordinal), entity.Start);
            Assert.AreEqual(expectedText.Length, entity.Length);
            Assert.AreEqual(expectedConfidence, entity.Confidence, 0.001);
        }
    }
}
