using GraphRag.NLP.Chunking;

namespace GraphRag.NLP.Tests.Chunking
{
    [TestClass]
    public sealed class RegexPhraseChunkServiceTests
    {
        [TestMethod]
        public async Task ExtractChunksAsync_WithBlankText_ThrowsArgumentException()
        {
            IPhraseChunkService service = new RegexPhraseChunkService();

            await Assert.ThrowsExactlyAsync<ArgumentException>(
                () => service.ExtractChunksAsync("   "));
        }

        [TestMethod]
        public async Task ExtractChunksAsync_WithCanceledToken_ThrowsOperationCanceledException()
        {
            IPhraseChunkService service = new RegexPhraseChunkService();
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            await Assert.ThrowsExactlyAsync<OperationCanceledException>(
                () => service.ExtractChunksAsync("Microsoft Graph powers search.", cancellationTokenSource.Token));
        }

        [TestMethod]
        public async Task ExtractChunksAsync_ExtractsCapitalizedPhrasesWithConnectors()
        {
            IPhraseChunkService service = new RegexPhraseChunkService();
            const string text = "University of Washington partners with City of London. Research and Development teams coordinate releases.";

            IReadOnlyList<PhraseChunk> chunks = await service.ExtractChunksAsync(text);

            Assert.HasCount(3, chunks);
            AssertChunk(chunks, text, "University of Washington");
            AssertChunk(chunks, text, "City of London");
            AssertChunk(chunks, text, "Research and Development");
        }

        [TestMethod]
        public async Task ExtractChunksAsync_ReturnsEmptyListForLowercaseOnlyText()
        {
            IPhraseChunkService service = new RegexPhraseChunkService();

            IReadOnlyList<PhraseChunk> chunks = await service.ExtractChunksAsync(
                "this sentence contains no capitalized phrase candidates.");

            Assert.IsEmpty(chunks);
        }

        [TestMethod]
        public async Task ExtractChunksAsync_PreservesApostrophesHyphensAndEmbeddedDigitsInChunkText()
        {
            IPhraseChunkService service = new RegexPhraseChunkService();
            const string text = "O'Reilly Media introduced GPT-4. Windows11 debuted at Build.";

            IReadOnlyList<PhraseChunk> chunks = await service.ExtractChunksAsync(text);

            Assert.HasCount(4, chunks);
            AssertChunk(chunks, text, "O'Reilly Media");
            AssertChunk(chunks, text, "GPT-4");
            AssertChunk(chunks, text, "Windows11");
            AssertChunk(chunks, text, "Build");
        }

        private static void AssertChunk(
            IReadOnlyList<PhraseChunk> chunks,
            string sourceText,
            string expectedText)
        {
            PhraseChunk? chunk = chunks.FirstOrDefault(candidate => candidate.Text == expectedText);
            Assert.IsNotNull(chunk);
            Assert.AreEqual(sourceText.IndexOf(expectedText, StringComparison.Ordinal), chunk.Start);
            Assert.AreEqual(expectedText.Length, chunk.Length);
        }
    }
}
