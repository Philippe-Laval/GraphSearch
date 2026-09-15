using System.Text;

namespace GraphRag.Lucene;

/// <summary>
/// MarkdownChunker chunks Markdown by paragraphs and then groups paragraphs 
/// up to an approximate character limit. 
/// Token-based chunking is preferable when the chunks will 
/// later be sent to an embedding or language model.
/// </summary>
public static class MarkdownChunker
{
    public static IReadOnlyList<string> Chunk(
        string markdown,
        int maximumCharacters,
        int overlapCharacters)
    {
        ArgumentNullException.ThrowIfNull(markdown);

        if (maximumCharacters <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maximumCharacters));
        }

        if (overlapCharacters < 0 ||
            overlapCharacters >= maximumCharacters)
        {
            throw new ArgumentOutOfRangeException(
                nameof(overlapCharacters));
        }

        var normalized = markdown.ReplaceLineEndings("\n");

        var paragraphs = normalized.Split(
            ["\n\n"],
            StringSplitOptions.RemoveEmptyEntries |
            StringSplitOptions.TrimEntries);

        var chunks = new List<string>();
        var builder = new StringBuilder();

        foreach (var paragraph in paragraphs)
        {
            if (builder.Length > 0 &&
                builder.Length + paragraph.Length + 2 >
                maximumCharacters)
            {
                AddChunk(chunks, builder);

                var overlap = GetOverlap(
                    builder.ToString(),
                    overlapCharacters);

                builder.Clear();

                if (overlap.Length > 0)
                {
                    builder.Append(overlap);
                    builder.Append("\n\n");
                }
            }

            // A very large paragraph is split directly.
            if (paragraph.Length > maximumCharacters &&
                builder.Length == 0)
            {
                SplitLargeParagraph(
                    chunks,
                    paragraph,
                    maximumCharacters,
                    overlapCharacters);

                continue;
            }

            if (builder.Length > 0)
            {
                builder.Append("\n\n");
            }

            builder.Append(paragraph);
        }

        AddChunk(chunks, builder);
        return chunks;
    }

    /// <summary>
    /// Splits a paragraph into overlapping chunks of specified maximum size and adds them to the collection.
    /// </summary>
    /// <param name="chunks">The collection to which the chunks will be added.</param>
    /// <param name="paragraph">The paragraph to split into chunks.</param>
    /// <param name="maximumCharacters">The maximum number of characters per chunk.</param>
    /// <param name="overlapCharacters">The number of overlapping characters between consecutive chunks.</param>
    private static void SplitLargeParagraph(
        ICollection<string> chunks,
        string paragraph,
        int maximumCharacters,
        int overlapCharacters)
    {
        var step = maximumCharacters - overlapCharacters;

        for (var offset = 0;
             offset < paragraph.Length;
             offset += step)
        {
            var length = Math.Min(
                maximumCharacters,
                paragraph.Length - offset);

            var value = paragraph
                .Substring(offset, length)
                .Trim();

            if (value.Length > 0)
            {
                chunks.Add(value);
            }
        }
    }

    /// <summary>
    /// Gets the overlapping text from the end of a chunk.
    /// </summary>
    /// <param name="text">The text from which to extract the overlap.</param>
    /// <param name="overlapCharacters">The number of overlapping characters.</param>
    /// <returns>The overlapping text.</returns>
    private static string GetOverlap(
        string text,
        int overlapCharacters)
    {
        if (overlapCharacters == 0)
        {
            return string.Empty;
        }

        var start = Math.Max(
            0,
            text.Length - overlapCharacters);

        // Try to begin the overlap at a word boundary.
        var boundary = text.IndexOf(' ', start);

        if (boundary >= 0 && boundary < text.Length - 1)
        {
            start = boundary + 1;
        }

        return text[start..].Trim();
    }

    /// <summary>
    /// Adds the trimmed content of the StringBuilder to the collection if not empty.
    /// </summary>
    /// <param name="chunks">The collection to which the chunk is added.</param>
    /// <param name="builder">The StringBuilder containing the content to add.</param>
    private static void AddChunk(
        ICollection<string> chunks,
        StringBuilder builder)
    {
        var value = builder.ToString().Trim();

        if (value.Length > 0)
        {
            chunks.Add(value);
        }
    }
}