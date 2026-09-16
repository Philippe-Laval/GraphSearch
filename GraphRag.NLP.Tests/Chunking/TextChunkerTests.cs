using GraphRag.NLP.Chunking;

namespace GraphRag.NLP.Tests.Chunking
{
    [TestClass]
    public sealed class TextChunkerTests
    {
        [TestMethod]
        public void Split_WithBlankText_ThrowsArgumentException()
        {
            Assert.ThrowsExactly<ArgumentException>(() => TextChunker.Split("   "));
        }

        [TestMethod]
        public void Split_WithNonPositiveMaximumCharacters_ThrowsArgumentOutOfRangeException()
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => TextChunker.Split("abc", maximumCharacters: 0, overlapCharacters: 0));
        }

        [TestMethod]
        public void Split_WithInvalidOverlap_ThrowsArgumentOutOfRangeException()
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => TextChunker.Split("abc", maximumCharacters: 10, overlapCharacters: -1));

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => TextChunker.Split("abc", maximumCharacters: 10, overlapCharacters: 10));
        }

        [TestMethod]
        public void Split_WithShortText_ReturnsSingleTrimmedChunk()
        {
            IReadOnlyList<string> chunks = TextChunker.Split(
                "  Hello world.  ",
                maximumCharacters: 50,
                overlapCharacters: 0);

            Assert.HasCount(1, chunks);
            Assert.AreEqual("Hello world.", chunks[0]);
        }

        [TestMethod]
        public void Split_PrefersParagraphBoundaries_WhenPossible()
        {
            const string text = "Alpha Beta.\n\nGamma Delta.";

            IReadOnlyList<string> chunks = TextChunker.Split(
                text,
                maximumCharacters: 20,
                overlapCharacters: 0);

            Assert.HasCount(2, chunks);
            Assert.AreEqual("Alpha Beta.", chunks[0]);
            Assert.AreEqual("Gamma Delta.", chunks[1]);
        }

        [TestMethod]
        public void Split_CreatesOverlappingChunks_WhenOverlapIsConfigured()
        {
            IReadOnlyList<string> chunks = TextChunker.Split(
                "ABCDEFGHIJKLMNO",
                maximumCharacters: 8,
                overlapCharacters: 3);

            Assert.HasCount(3, chunks);
            Assert.AreEqual("ABCDEFGH", chunks[0]);
            Assert.AreEqual("FGHIJKLM", chunks[1]);
            Assert.AreEqual("KLMNO", chunks[2]);
        }
    }
}
