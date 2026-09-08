namespace GraphSearch.Library.Query.Analysis.EntityExtraction;

/// <summary>
/// Factory class for creating instances of <see cref="DictionaryEntityExtractor"/>.
/// </summary>
public static class DictionaryEntityExtractorFactory
{
    /// <summary>
    /// Creates an instance of <see cref="DictionaryEntityExtractor"/> with the specified entity definitions.
    /// </summary>
    /// <param name="entities">The entity definitions to initialize the extractor with.</param>
    /// <returns></returns>
    public static IEntityExtractor Create(
        IEnumerable<EntityDefinition> entities)
    {
        return new DictionaryEntityExtractor(entities);
    }

    /// <summary>
    /// Creates an instance of <see cref="DictionaryEntityExtractor"/> with the specified entity definitions.
    /// </summary>
    /// <param name="entities">The entity definitions to initialize the extractor with.</param>
    /// <returns>An instance of <see cref="DictionaryEntityExtractor"/>.</returns>
    public static IEntityExtractor Create(
        params EntityDefinition[] entities)
    {
        return new DictionaryEntityExtractor(entities);
    }

    /// <summary>
    /// Creates an instance of <see cref="DictionaryEntityExtractor"/> with the specified entity definitions.
    /// </summary>
    /// <returns>An instance of <see cref="DictionaryEntityExtractor"/>.</returns>
    public static IEntityExtractor Create()
    {
        EntityDefinition[] entities = [
            new("Apple", "Organization"),
            new("Microsoft", "Organization"),
            new(".NET", "Technology"),
            new(".NET 10", "Technology"),
            new("C#", "ProgrammingLanguage"),
            new("C", "ProgrammingLanguage"),
            new("C++", "ProgrammingLanguage"),
            new("Java", "ProgrammingLanguage"),
            new("Python", "ProgrammingLanguage"),
            new("Rust", "ProgrammingLanguage"),
            new("Swift", "ProgrammingLanguage"),
            new("Windows", "OperatingSystem"),
            new("MacOS", "OperatingSystem"),
            new("Linux", "OperatingSystem")
        ];
        
        return new DictionaryEntityExtractor(entities);
    }
}