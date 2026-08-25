namespace GraphSearch.Query.Analysis;

public static class DictionaryEntityExtractorFactory
{
    public static IEntityExtractor Create(
        IEnumerable<EntityDefinition> entities)
    {
        return new DictionaryEntityExtractor(entities);
    }

    public static IEntityExtractor Create(
        params EntityDefinition[] entities)
    {
        return new DictionaryEntityExtractor(entities);
    }

    public static IEntityExtractor Create()
    {
        EntityDefinition[] entities = [
            new("Microsoft", "Organization"),
            new(".NET", "Technology"),
            new(".NET 10", "Technology"),
            new("C#", "ProgrammingLanguage"),
            new("Linux", "OperatingSystem")
        ];
        
        return new DictionaryEntityExtractor(entities);
    }
}