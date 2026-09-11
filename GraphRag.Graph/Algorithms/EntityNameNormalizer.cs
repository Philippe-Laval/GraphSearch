using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace GraphRag.Graph.Algorithms;

/// <summary>
/// Level 1: deterministic normalization
/// Normalize casing, punctuation and whitespace.
/// 
/// Level 2: aliases returned by the LLM
///
/// If one node contains:
///
/// {
///  "name": "Large language model",
///  "aliases": ["LLM", "large-language model"]
/// }
/// 
/// Level 3: embeddings
///
/// Create an embedding from the node’s name and description:
///
/// Large language model | Neural language model trained on large text corpora
///
/// Then compare new nodes to existing nodes using cosine similarity.
///
/// A high similarity may suggest a duplicate, but it should not automatically prove identity.
/// For example, “SQL Server” and “PostgreSQL” may be semantically close without being the same entity.
///
/// Level 4: LLM disambiguation
///
/// For uncertain candidates, ask the LLM:
///
/// Do these two extracted topics refer to the same real-world concept in the supplied context?
///
/// Topic A: ...
/// Topic B: ...
///
/// Return SameEntity, RelatedButDifferent, or Unrelated.
/// 
/// </summary>
public static partial class EntityNameNormalizer
{
    public static string Normalize(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        string normalized = value
            .Normalize(NormalizationForm.FormD);

        var builder = new StringBuilder(normalized.Length);

        foreach (char character in normalized)
        {
            UnicodeCategory category =
                CharUnicodeInfo.GetUnicodeCategory(character);

            if (category != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(character);
            }
        }

        normalized = builder
            .ToString()
            .Normalize(NormalizationForm.FormC)
            .ToLowerInvariant();

        normalized = NonAlphaNumericRegex()
            .Replace(normalized, " ");

        return MultipleSpacesRegex()
            .Replace(normalized, " ")
            .Trim();
    }

    [GeneratedRegex(@"[^\p{L}\p{N}]+")]
    private static partial Regex NonAlphaNumericRegex();

    [GeneratedRegex(@"\s+")]
    private static partial Regex MultipleSpacesRegex();
}
