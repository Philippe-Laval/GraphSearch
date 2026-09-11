using GraphRag.Core.Models;
using GraphRag.Core.Interfaces;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace GraphRag.Indexing.Renderers;

/// <summary>
/// Renders an EmbeddingDocument as a YAML string.
/// Le YAML est celui destiné pour le calcul d'embedding (vecteurs)
/// Calcul des embeddings et versioning (structure stable, déterministe).
/// </summary>
public sealed class YamlRenderer : IEmbeddingDocumentRenderer
{
    private readonly ISerializer _serializer =
        new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

    public string Format => "yaml";

    /// <summary>
    /// Creates a YAML representation of the given EmbeddingDocument.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="document"></param>
    /// <returns></returns>
    public string Render<T>(EmbeddingDocument<T> document)
        => _serializer.Serialize(document);
}

