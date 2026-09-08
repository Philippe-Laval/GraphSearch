namespace GraphSearch.Library.Query.Analysis;

/// <summary>
/// Detects the (BCP-47) language of a query.
/// </summary>
public interface ILanguageDetector
{
    /// <summary>
    /// Returns a BCP-47 language code ("en", "fr", "de", ...) or null when unknown.
    /// </summary>
    string? Detect(string text);
}
