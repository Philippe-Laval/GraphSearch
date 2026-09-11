using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Graph.Algorithms;

/// <summary>
/// For a long document, split it into overlapping chunks
/// </summary>
public static class TextChunker
{
    /// <summary>
    /// Divise un texte en segments de taille limitée avec chevauchement, 
    /// en privilégiant les coupures aux limites de paragraphe.
    /// </summary>
    /// <param name="text">Texte à diviser.</param>
    /// <param name="maximumCharacters">Nombre maximum de caractères par segment.</param>
    /// <param name="overlapCharacters">Nombre de caractères de chevauchement entre segments consécutifs.</param>
    /// <returns>Liste en lecture seule des segments de texte.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maximumCharacters"/> est inférieur ou égal à zéro, ou <paramref name="overlapCharacters"/> est
    /// négatif ou supérieur ou égal à <paramref name="maximumCharacters"/>.</exception>
    public static IReadOnlyList<string> Split(
        string text,
        int maximumCharacters = 12_000,
        int overlapCharacters = 500)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

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

        var chunks = new List<string>();
        var position = 0;

        while (position < text.Length)
        {
            int length = Math.Min(
                maximumCharacters,
                text.Length - position);

            int end = position + length;

            if (end < text.Length)
            {
                int paragraphBreak =
                    text.LastIndexOf(
                        "\n\n",
                        end,
                        length,
                        StringComparison.Ordinal);

                if (paragraphBreak > position)
                {
                    end = paragraphBreak;
                }
            }

            string chunk = text[position..end].Trim();

            if (chunk.Length > 0)
            {
                chunks.Add(chunk);
            }

            if (end >= text.Length)
            {
                break;
            }

            position = Math.Max(
                position + 1,
                end - overlapCharacters);
        }

        return chunks;
    }
}
