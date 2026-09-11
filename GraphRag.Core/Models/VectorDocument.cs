using Microsoft.Extensions.AI;

namespace GraphRag.Core.Models;

public class VectorDocument
{
    private string entityHash;
    private Embedding<float> embedding;
    private string text;

    public VectorDocument(string entityHash, Embedding<float> embedding, string text)
    {
        this.entityHash = entityHash;
        this.embedding = embedding;
        this.text = text;
    }
}