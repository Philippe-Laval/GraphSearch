using GraphRag.NLP.Models;

namespace GraphRag.NLP.EntityExtraction
{
    /// <summary>
    /// Extracts entities from text.
    /// </summary>
    public interface IEntityExtractor
    {
        /// <summary>
        /// Extracts entities from the given text.
        /// </summary>
        /// <param name="text">The text from which to extract entities.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>The extracted entities.</returns>
        Task<IReadOnlyList<ExtractedEntity>> ExtractAsync(
            string text,
            CancellationToken cancellationToken = default);
    }
}
