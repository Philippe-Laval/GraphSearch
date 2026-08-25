using GraphSearch.Query.Embeddings;

namespace GraphSearch.Query.Resolution;

/*
 What I would actually store in Chroma
   
   I wouldn't store only:

   .NET 10
   
   I'd create an embedding document:
   
   Type: Technology
   Name: .NET 10
   Aliases: DotNet 10, Microsoft .NET 10
   Description: Cross-platform runtime and development platform...
   
   Then metadata:
   
   {
       "nodeId": 1842,
       "type": "Technology",
       "name": ".NET 10"
   }
   
   The embedding represents the meaning of the entity, 
   while metadata gives you deterministic information.
 */

/*
public sealed class ChromaEntityIndex : IEntityIndex
{
    private readonly ChromaClient _client;
    private readonly string _collectionId;
    private readonly IEmbeddingService _embeddingService;

    public ChromaEntityIndex(
        ChromaClient client,
        string collectionId,
        IEmbeddingService embeddingService)
    {
        _client = client;
        _collectionId = collectionId;
        _embeddingService = embeddingService;
    }

    public async Task<IReadOnlyList<EntityCandidate>> SearchAsync(
        string text,
        string? entityType,
        int topK,
        CancellationToken cancellationToken = default)
    {
        var embedding =
            await _embeddingService.EmbedAsync(
                text,
                cancellationToken);

        var result =
            await _client.QueryEmbeddingsAsync(
                _collectionId,
                [embedding],
                topK,
                [
                    "metadatas",
                    "distances"
                ],
                cancellationToken);

        return ConvertResults(result);
    }

    private static IReadOnlyList<EntityCandidate>
        ConvertResults(
            ChromaQueryResultModel result)
    {
        // See note below about the exact model
        // shape of the installed SK connector version.

        var candidates =
            new List<EntityCandidate>();

        // Map Chroma metadata + distance
        // into EntityCandidate here.

        return candidates;
    }
}
*/