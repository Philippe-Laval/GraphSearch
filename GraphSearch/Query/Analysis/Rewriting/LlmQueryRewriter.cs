using System.Text.Json;
using System.Text.Json.Serialization;

namespace GraphSearch.Library.Query.Analysis.Rewriting;

/// <summary>
/// LLM-backed paraphraser. Asks an <see cref="IChatCompletionClient"/> to
/// produce N diverse rewordings of the input while preserving intent and
/// entities.
///
/// <para>
/// Expected reply is a bare JSON array of strings; the parser tolerates
/// <c>```json</c> fences and any leading/trailing prose. On any error or
/// empty output the rewriter fails <b>open</b> and returns an empty list.
/// </para>
///
/// <para>
/// Best used behind <see cref="CachingQueryRewriter"/> (queries repeat) and
/// composed with cheaper rewriters via <see cref="CompositeQueryRewriter"/>.
/// </para>
/// </summary>
public sealed class LlmQueryRewriter : IQueryRewriter
{
    private const string DefaultPromptTemplate =
        """
        You paraphrase user queries for a GraphRAG retrieval system.
        Produce {{COUNT}} diverse rewordings of the user query that preserve
        its meaning and any named entities. Do NOT invent new facts or entities.
        Do NOT translate the query to another language.
        Reply with a JSON array of strings only, no prose.

        User query:
        {{QUERY}}
        """;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly IChatCompletionClient _client;
    private readonly string _promptTemplate;
    private readonly int _maxRewrites;
    private readonly double _weight;

    public LlmQueryRewriter(
        IChatCompletionClient client,
        int maxRewrites = 4,
        double weight = 1.0,
        string? promptTemplate = null)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxRewrites);

        _client = client;
        _maxRewrites = maxRewrites;
        _weight = weight;
        _promptTemplate = promptTemplate ?? DefaultPromptTemplate;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<QueryRewrite>> RewriteAsync(
        string normalizedQuery,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(normalizedQuery);

        // Compose the prompt with the query and count
        var prompt = _promptTemplate
            .Replace("{{QUERY}}", normalizedQuery, StringComparison.Ordinal)
            .Replace("{{COUNT}}", _maxRewrites.ToString(System.Globalization.CultureInfo.InvariantCulture), StringComparison.Ordinal);

        string raw;
        try
        {
            // Call the LLM client to get the raw completion
            raw = await _client.CompleteAsync(prompt, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            return Array.Empty<QueryRewrite>();
        }

        // Extract the JSON array from the raw response
        var payload = ExtractJsonArray(raw);

        // Parse the JSON array into a list of strings
        List<string>? parsed;
        try
        {
            parsed = JsonSerializer.Deserialize<List<string>>(payload, JsonOptions);
        }
        catch (JsonException)
        {
            return Array.Empty<QueryRewrite>();
        }

        if (parsed is null || parsed.Count == 0)
        {
            return Array.Empty<QueryRewrite>();
        }

        // Use a HashSet to track unique paraphrases, ignoring case
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { normalizedQuery };
        var result = new List<QueryRewrite>(capacity: Math.Min(parsed.Count, _maxRewrites));

        foreach (var text in parsed)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                continue;
            }

            // Trim the text and check for uniqueness
            var trimmed = text.Trim();
            if (!seen.Add(trimmed))
            {
                continue;
            }

            // Add the unique paraphrase to the result list
            result.Add(new QueryRewrite(
                Text: trimmed,
                Kind: RewriteKind.LlmParaphrase,
                Weight: _weight));

            if (result.Count >= _maxRewrites)
            {
                break;
            }
        }

        return result;
    }

    
    private static string ExtractJsonArray(string raw)
    {
        // Tolerant extraction: strip prose / code fences and return the first [...]

        if (string.IsNullOrWhiteSpace(raw))
        {
            return "[]";
        }

        var start = raw.IndexOf('[');
        var end = raw.LastIndexOf(']');
        
        if (start < 0 || end < 0 || end <= start)
        {
            return "[]";
        }
        
        return raw.Substring(start, end - start + 1);
    }
}
