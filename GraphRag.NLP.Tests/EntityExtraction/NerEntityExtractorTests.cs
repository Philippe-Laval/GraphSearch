using GraphRag.NLP.EntityExtraction;
using GraphRag.NLP.Models;
using GraphRag.NLP.Ner;

namespace GraphRag.NLP.Tests.EntityExtraction
{
    [TestClass]
    public sealed class NerEntityExtractorTests
    {
        [TestMethod]
        public void Constructor_WithNullNerService_ThrowsArgumentNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new NerEntityExtractor(null!));
        }

        [TestMethod]
        public async Task ExtractAsync_WithBlankText_ThrowsArgumentException()
        {
            IEntityExtractor extractor = new NerEntityExtractor(new FakeNerService(CreateAnalysis()));

            await Assert.ThrowsExactlyAsync<ArgumentException>(
                () => extractor.ExtractAsync("   "));
        }

        [TestMethod]
        public async Task ExtractAsync_WithoutTypeAllowList_ReturnsAllEntitiesFromNerService()
        {
            NerAnalysis analysis = CreateAnalysis();
            IEntityExtractor extractor = new NerEntityExtractor(new FakeNerService(analysis));

            IReadOnlyList<ExtractedEntity> entities = await extractor.ExtractAsync("Microsoft hired Ada Lovelace in London.");

            Assert.HasCount(3, entities);
            Assert.AreEqual("Microsoft", entities[0].Text);
            Assert.AreEqual("Organization", entities[0].Type);
            Assert.AreEqual("Ada Lovelace", entities[1].Text);
            Assert.AreEqual("Person", entities[1].Type);
            Assert.AreEqual("London", entities[2].Text);
            Assert.AreEqual("Location", entities[2].Type);
        }

        [TestMethod]
        public async Task ExtractAsync_WithTypeAllowList_FiltersEntitiesCaseInsensitively()
        {
            NerAnalysis analysis = new(
                Lemma: null,
                Entities:
                [
                    new ExtractedEntity("Microsoft", "Organization", 0, 9, 0.98),
                    new ExtractedEntity("Ada Lovelace", "Person", 18, 12, 0.97),
                    new ExtractedEntity("London", "Location", 34, 6, 0.96),
                    new ExtractedEntity("Unknown", null, 42, 7, 0.40)
                ],
                RootVerbs: [],
                InterrogativeLemma: null);

            IEntityExtractor extractor = new NerEntityExtractor(
                new FakeNerService(analysis),
                typeAllowList: ["organization", "LOCATION"]);

            IReadOnlyList<ExtractedEntity> entities = await extractor.ExtractAsync("Microsoft hired Ada Lovelace in London.");

            Assert.HasCount(2, entities);
            Assert.AreEqual("Microsoft", entities[0].Text);
            Assert.AreEqual("Organization", entities[0].Type);
            Assert.AreEqual("London", entities[1].Text);
            Assert.AreEqual("Location", entities[1].Type);
        }

        [TestMethod]
        public async Task ExtractAsync_WhenNerServiceFails_PropagatesException()
        {
            IEntityExtractor extractor = new NerEntityExtractor(
                new FakeNerService((_, _) => throw new InvalidOperationException("NER backend unavailable.")));

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                () => extractor.ExtractAsync("Microsoft hired Ada Lovelace in London."));
        }

        [TestMethod]
        public async Task ExtractAsync_PassesCancellationTokenToNerService()
        {
            bool receivedCanceledToken = false;
            IEntityExtractor extractor = new NerEntityExtractor(
                new FakeNerService((_, cancellationToken) =>
                {
                    receivedCanceledToken = cancellationToken.IsCancellationRequested;
                    cancellationToken.ThrowIfCancellationRequested();
                    return CreateAnalysis();
                }));

            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            await Assert.ThrowsExactlyAsync<OperationCanceledException>(
                () => extractor.ExtractAsync("Microsoft hired Ada Lovelace in London.", cancellationTokenSource.Token));

            Assert.IsTrue(receivedCanceledToken);
        }

        private static NerAnalysis CreateAnalysis() =>
            new(
                Lemma: "microsoft hire ada lovelace in london",
                Entities:
                [
                    new ExtractedEntity("Microsoft", "Organization", 0, 9, 0.98),
                    new ExtractedEntity("Ada Lovelace", "Person", 16, 12, 0.97),
                    new ExtractedEntity("London", "Location", 32, 6, 0.96)
                ],
                RootVerbs: ["hire"],
                InterrogativeLemma: null);

        private sealed class FakeNerService : INerService
        {
            private readonly Func<string, CancellationToken, NerAnalysis> _analyze;

            public FakeNerService(NerAnalysis analysis)
                : this((_, _) => analysis)
            {
            }

            public FakeNerService(Func<string, CancellationToken, NerAnalysis> analyze)
            {
                ArgumentNullException.ThrowIfNull(analyze);
                _analyze = analyze;
            }

            public Task<NerAnalysis> AnalyzeAsync(
                string text,
                CancellationToken cancellationToken = default)
            {
                return Task.FromResult(_analyze(text, cancellationToken));
            }
        }
    }
}
