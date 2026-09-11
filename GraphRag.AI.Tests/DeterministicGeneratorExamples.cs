using GraphRag.Core.Models;
using GraphRag.Indexing.Generators;

namespace GraphRag.AI.Tests;

/// <summary>
/// Exemples d'utilisation des générateurs de texte déterministes.
/// Ces exemples peuvent être utilisés comme référence pour comprendre comment
/// utiliser les générateurs dans votre propre code.
/// </summary>
public static class DeterministicGeneratorExamples
{
    /// <summary>
    /// Exemple : Générer une documentation Markdown pour un nœud
    /// </summary>
    public static async Task<string> GenerateNodeDocumentation()
    {
        var node = new GraphNode(
            Id: Guid.NewGuid(),
            Type: "Technology",
            Label: ".NET",
            Description: "Une plateforme de développement open-source créée par Microsoft pour construire des applications modernes",
            Properties: new Dictionary<string, object?>
            {
                { "ReleaseYear", 2002 },
                { "OpenSource", true }
            }
        );

        var generator = new NodeDeterministicTextGenerator(DeterministicTextFormat.Markdown);
        return await generator.GenerateAsync(node);
    }

    /// <summary>
    /// Exemple : Exporter un nœud en format YAML pour intégration
    /// </summary>
    public static async Task<string> ExportNodeToYaml()
    {
        var node = new GraphNode(
            Id: Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
            Type: "Concept",
            Label: "Microservices",
            Description: "Architecture logicielle où l'application est décomposée en services indépendants",
            Properties: new Dictionary<string, object?>
            {
                { "Benefits", new[] { "Scalability", "Resilience", "Independent Deployment" } },
                { "Challenges", new[] { "Complexity", "Distributed Systems" } }
            }
        );

        var generator = new NodeDeterministicTextGenerator(DeterministicTextFormat.Yaml);
        return await generator.GenerateAsync(node);
    }

    /// <summary>
    /// Exemple : Générer une documentation pour une relation entre deux concepts
    /// </summary>
    public static async Task<string> GenerateEdgeDocumentation()
    {
        var edge = new GraphEdge(
            Id: Guid.NewGuid(),
            SourceId: Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
            TargetId: Guid.Parse("550e8400-e29b-41d4-a716-446655440001"),
            Type: "IMPLEMENTS",
            Properties: new Dictionary<string, object?>
            {
                { "Description", "Implémente le concept" }
            }
        );

        var generator = new EdgeDeterministicTextGenerator(DeterministicTextFormat.Markdown);
        return await generator.GenerateAsync(edge);
    }

    /// <summary>
    /// Exemple : Créer un graphe de connaissances complet et le documenter
    /// </summary>
    public static async Task<string> GenerateKnowledgeGraphDocumentation()
    {
        // Créer les nœuds
        var dotnet = new GraphNode(
            Id: Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Type: "Technology",
            Label: ".NET",
            Description: "Plateforme de développement Microsoft",
            Properties: new Dictionary<string, object?>
            {
                { "ReleaseYear", 2002 },
                { "OpenSource", true }
            }
        );

        var csharp = new GraphNode(
            Id: Guid.Parse("00000000-0000-0000-0000-000000000002"),
            Type: "Language",
            Label: "C#",
            Description: "Langage de programmation orienté objet",
            Properties: new Dictionary<string, object?>
            {
                { "DesignedBy", "Microsoft" },
                { "FirstAppeared", 2000 }
            }
        );

        var aspnetCore = new GraphNode(
            Id: Guid.Parse("00000000-0000-0000-0000-000000000003"),
            Type: "Framework",
            Label: "ASP.NET Core",
            Description: "Framework web pour .NET",
            Properties: new Dictionary<string, object?>
            {
                { "ReleaseYear", 2016 },
                { "CrossPlatform", true }
            }
        );

        // Créer les arêtes
        var edge1 = new GraphEdge(
            Id: Guid.Parse("00000000-0000-0000-0000-000000000010"),
            SourceId: dotnet.Id,
            TargetId: csharp.Id,
            Type: "SUPPORTS",
            Properties: new Dictionary<string, object?>
            {
                { "Description", "Supporte le langage C#" }
            }
        );

        var edge2 = new GraphEdge(
            Id: Guid.Parse("00000000-0000-0000-0000-000000000011"),
            SourceId: aspnetCore.Id,
            TargetId: dotnet.Id,
            Type: "BUILT_ON",
            Properties: new Dictionary<string, object?>
            {
                { "Description", "Construit sur .NET" }
            }
        );

        // Créer le sous-graphe
        var subGraph = new SubGraph
        {
            Nodes = new[] { dotnet, csharp, aspnetCore },
            Edges = new[] { edge1, edge2 }
        };

        // Générer la documentation
        var generator = new SubGraphDeterministicTextGenerator(DeterministicTextFormat.Markdown);
        return await generator.GenerateAsync(subGraph);
    }

    /// <summary>
    /// Exemple : Exporter un graphe complet en YAML pour traitement automatisé
    /// </summary>
    public static async Task<string> ExportKnowledgeGraphToYaml()
    {
        var node1 = new GraphNode(
            Id: Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Type: "Concept",
            Label: "Intelligence Artificielle",
            Description: "Simulation de l'intelligence humaine par des machines",
            Properties: new Dictionary<string, object?>
            {
                { "Field", "Computer Science" },
                { "Applications", new[] { "Natural Language Processing", "Computer Vision", "Robotics" } }
            }
        );

        var node2 = new GraphNode(
            Id: Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Type: "Concept",
            Label: "Machine Learning",
            Description: "Sous-domaine de l'IA basé sur l'apprentissage à partir de données",
            Properties: new Dictionary<string, object?>
            {
                { "Field", "Artificial Intelligence" },
                { "Techniques", new[] { "Supervised Learning", "Unsupervised Learning", "Reinforcement Learning" } }
            }
        );

        var edge = new GraphEdge(
            Id: Guid.Parse("33333333-3333-3333-3333-333333333333"),
            SourceId: node1.Id,
            TargetId: node2.Id,
            Type: "INCLUDES",
            Properties: new Dictionary<string, object?>
            {
                { "Description", "L'intelligence artificielle inclut le machine learning" }
            }
        );

        var subGraph = new SubGraph
        {
            Nodes = new[] { node1, node2 },
            Edges = new[] { edge }
        };

        var generator = new SubGraphDeterministicTextGenerator(DeterministicTextFormat.Yaml);
        return await generator.GenerateAsync(subGraph);
    }

    /// <summary>
    /// Exemple : Générer du texte pour indexation vectorielle
    /// </summary>
    public static async Task<string> GenerateTextForEmbedding()
    {
        var node = new GraphNode(
            Id: Guid.NewGuid(),
            Type: "Concept",
            Label: "GraphRAG",
            Description: "Technique combinant les graphes de connaissances avec la génération augmentée par récupération",
            Properties: new Dictionary<string, object?>
            {
                { "Purpose", "Améliorer les réponses des modèles de langage" },
                { "Components", new[] { "Knowledge Graph", "Retrieval-Augmented Generation" } }
            }
        );

        // Pour l'indexation, le format Markdown est souvent préféré
        // car il contient plus de contexte structuré
        var generator = new NodeDeterministicTextGenerator(DeterministicTextFormat.Markdown);
        var text = await generator.GenerateAsync(node);

        // Ce texte peut ensuite être passé à un service d'embedding
        // var embedding = await embeddingService.GenerateAsync(text);

        return text;
    }

    /// <summary>
    /// Exemple : Comparer la stabilité entre plusieurs générations
    /// </summary>
    public static async Task<bool> VerifyDeterminism()
    {
        var node = new GraphNode(
            Id: Guid.Parse("99999999-9999-9999-9999-999999999999"),
            Type: "Test",
            Label: "Determinism Test",
            Description: "Ce nœud teste le déterminisme",
            Properties: new Dictionary<string, object?>
            {
                { "TestProperty", "TestValue" }
            }
        );

        var generator = new NodeDeterministicTextGenerator(DeterministicTextFormat.Markdown);

        // Générer 100 fois et vérifier que c'est toujours identique
        string? firstResult = null;

        for (int i = 0; i < 100; i++)
        {
            var result = await generator.GenerateAsync(node);

            if (firstResult == null)
            {
                firstResult = result;
            }
            else if (firstResult != result)
            {
                return false; // Non déterministe !
            }
        }

        return true; // Déterministe ✓
    }

    /// <summary>
    /// Exemple : Générer un graphe vide avec gestion appropriée
    /// </summary>
    public static async Task<string> GenerateEmptyGraphDocumentation()
    {
        var emptyGraph = new SubGraph
        {
            Nodes = Array.Empty<GraphNode>(),
            Edges = Array.Empty<GraphEdge>()
        };

        var generator = new SubGraphDeterministicTextGenerator(DeterministicTextFormat.Markdown);
        return await generator.GenerateAsync(emptyGraph);
    }
}
