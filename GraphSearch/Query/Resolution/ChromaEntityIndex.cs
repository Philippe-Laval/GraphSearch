using Chroma;
using ChromaDB.Library;
using GraphSearch.Library.Embeddings;

namespace GraphSearch.Library.Query.Resolution;

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


public sealed class ChromaEntityIndex : IEntityIndex
{
    private readonly ChromaDBClient _client;
    private readonly string _tenant;
    private readonly string _database;
    private readonly string _collectionId;
    private readonly IEmbeddingService _embeddingService;

    public ChromaEntityIndex(
        ChromaDBClient client,
        string tenant,
        string database,
        string collectionId,
        IEmbeddingService embeddingService)
    {
        _client = client;
        _collectionId = collectionId;
        _database = database;
        _tenant = tenant;
        _embeddingService = embeddingService;
    }

    public async Task<IReadOnlyList<EntityCandidate>> SearchAsync(
        string text,
        string? entityType,
        int topK,
        CancellationToken cancellationToken = default)
    {
        var embedding = await _embeddingService.EmbedAsync(text, cancellationToken);

        IList<IList<float>> embeddings = new List<IList<float>>
            {
                new List<float>(embedding.ToArray())
            };

        // Include all fields in the result, but you can choose to include only the fields you need.
        var include = new List<Include> { Include.Documents,
                    Include.Embeddings,
                    Include.Distances,
                    Include.Metadatas,
                    Include.Uris };

        IList<IList<ChromaDbDocument>> result =
            await _client.CollectionQueryAsync(_collectionId, embeddings, include, null, topK, null, null, null, null,
            _database, _tenant, cancellationToken);

        return ConvertResults(result);
    }

    private static IReadOnlyList<EntityCandidate> ConvertResults(IList<IList<ChromaDbDocument>> result)
    {
        // See note below about the exact model
        // shape of the installed SK connector version.

        var candidates = new List<EntityCandidate>();

        // Map Chroma metadata + distance into EntityCandidate here.
        IList<ChromaDbDocument> docs = result[0];

        foreach (var doc in docs)
        {
            if (doc.Metadata != null &&
                doc.Metadata.TryGetValue("nodeId", out var nodeIdObj) &&
                doc.Metadata.TryGetValue("name", out var nameObj) &&
                doc.Metadata.TryGetValue("type", out var typeObj) &&
                nodeIdObj is long nodeId &&
                nameObj is string name &&
                typeObj is string type)
            {
                // TODO : create a unit test to see if the score is calculated correctly.
                // (1.0 - doc.Distance.Value) or doc.Distance.Value

                // The score is a value between 0 and 1, where 1 is the best match and 0 is the worst match.
                // The score is calculated as 1.0 - distance, where distance is a value between 0 and 1.
                // But ChromaDB has a cosine distance metric, which is already 1 - cosine similarity.
                double score = doc.Distance.HasValue ? doc.Distance.Value : 0.0;
                candidates.Add(new EntityCandidate(nodeId, name, type, score));
            }
        }

        return candidates;
    }
}
