using GraphRag.Core.Models;
using GraphRag.Indexing.Generators;

namespace GraphRag.AI.Tests;

[TestClass]
public sealed class DeterministicTextGeneratorTests
{
    [TestMethod]
    public async Task NodeDeterministicTextGenerator_Markdown_ShouldGenerateStableOutput()
    {
        // Arrange
        var node = new GraphNode(
            Id: Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Type: "Concept",
            Label: "Machine Learning",
            Description: "Un domaine de l'intelligence artificielle",
            Properties: new Dictionary<string, object?>
            {
                { "Field", "Artificial Intelligence" },
                { "Techniques", new[] { "Supervised Learning", "Unsupervised Learning", "Reinforcement Learning" } }
            });

        var generator = new NodeDeterministicTextGenerator(DeterministicTextFormat.Markdown);

        // Act
        var result1 = await generator.GenerateAsync(node);
        var result2 = await generator.GenerateAsync(node);

        // Assert
        Assert.AreEqual(result1, result2, "Les générations doivent être identiques (déterministes)");
        Assert.IsTrue(result1.Contains("# Machine Learning"));
        Assert.IsTrue(result1.Contains("**Type**: Concept"));
        Assert.IsTrue(result1.Contains("00000000-0000-0000-0000-000000000001"));
    }

    [TestMethod]
    public async Task NodeDeterministicTextGenerator_Yaml_ShouldGenerateStableOutput()
    {
        // Arrange
        var node = new GraphNode(
            Id: Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Type: "Concept",
            Label: "Machine Learning",
            Description: "Un domaine de l'intelligence artificielle",
            Properties: new Dictionary<string, object?>
            {
                { "Field", "Artificial Intelligence" },
                { "Techniques", new[] { "Supervised Learning", "Unsupervised Learning", "Reinforcement Learning" } }
            });

        var generator = new NodeDeterministicTextGenerator(DeterministicTextFormat.Yaml);

        // Act
        var result1 = await generator.GenerateAsync(node);
        var result2 = await generator.GenerateAsync(node);

        // Assert
        Assert.AreEqual(result1, result2, "Les générations doivent être identiques (déterministes)");
        Assert.IsTrue(result1.Contains("node:"));
        Assert.IsTrue(result1.Contains("type: \"Concept\""));
        Assert.IsTrue(result1.Contains("label: \"Machine Learning\""));
    }

    [TestMethod]
    public async Task EdgeDeterministicTextGenerator_Markdown_ShouldGenerateStableOutput()
    {
        // Arrange
        var edge = new GraphEdge(
            Id: Guid.Parse("00000000-0000-0000-0000-000000000001"),
            SourceId: Guid.Parse("00000000-0000-0000-0000-000000000002"),
            TargetId: Guid.Parse("00000000-0000-0000-0000-000000000003"),
            Type: "USES",
            Properties: new Dictionary<string, object?>
            {
                { "Description", "Utilise le concept" }
            });

        var generator = new EdgeDeterministicTextGenerator(DeterministicTextFormat.Markdown);

        // Act
        var result1 = await generator.GenerateAsync(edge);
        var result2 = await generator.GenerateAsync(edge);

        // Assert
        Assert.AreEqual(result1, result2, "Les générations doivent être identiques (déterministes)");
        Assert.IsTrue(result1.Contains("# Arête: USES"));
        Assert.IsTrue(result1.Contains("**Type**: USES"));
        Assert.IsTrue(result1.Contains("00000000-0000-0000-0000-000000000002"));
        Assert.IsTrue(result1.Contains("00000000-0000-0000-0000-000000000003"));
    }

    [TestMethod]
    public async Task EdgeDeterministicTextGenerator_Yaml_ShouldGenerateStableOutput()
    {
        // Arrange
        var edge = new GraphEdge(
            Id: Guid.Parse("00000000-0000-0000-0000-000000000001"),
            SourceId: Guid.Parse("00000000-0000-0000-0000-000000000002"),
            TargetId: Guid.Parse("00000000-0000-0000-0000-000000000003"),
            Type: "USES",
            Properties: new Dictionary<string, object?>
            {
                { "Description", "Utilise le concept" }
            });

        var generator = new EdgeDeterministicTextGenerator(DeterministicTextFormat.Yaml);

        // Act
        var result1 = await generator.GenerateAsync(edge);
        var result2 = await generator.GenerateAsync(edge);

        // Assert
        Assert.AreEqual(result1, result2, "Les générations doivent être identiques (déterministes)");
        Assert.IsTrue(result1.Contains("edge:"));
        Assert.IsTrue(result1.Contains("type: \"USES\""));
        Assert.IsTrue(result1.Contains("source_id:"));
        Assert.IsTrue(result1.Contains("target_id:"));
    }

    [TestMethod]
    public async Task SubGraphDeterministicTextGenerator_Markdown_ShouldGenerateStableOutputWithSortedElements()
    {
        // Arrange
        var node1 = new GraphNode(
            Id: Guid.Parse("00000000-0000-0000-0000-000000000003"),
            Type: "Concept",
            Label: "Deep Learning",
            Description: "Réseaux de neurones profonds",
            Properties: new Dictionary<string, object?>
            {
                { "Field", "Artificial Intelligence" },
                { "Techniques", new[] { "Neural Networks", "Deep Neural Networks" } }
            });

        var node2 = new GraphNode(
            Id: Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Type: "Concept",
            Label: "Machine Learning",
            Description: "Apprentissage automatique", 
            Properties: new Dictionary<string, object?>
            {
                { "Field", "Artificial Intelligence" },
                { "Techniques", new[] { "Supervised Learning", "Unsupervised Learning", "Reinforcement Learning" } }
            });

        var edge1 = new GraphEdge(
            Id: Guid.Parse("00000000-0000-0000-0000-000000000005"),
            SourceId: node2.Id,
            TargetId: node1.Id,
            Type: "INCLUDES",
            Properties: new Dictionary<string, object?>
            {
                { "Description", "Le machine learning inclut le deep learning" }
            });

        // Ordre aléatoire intentionnel pour tester le tri
        var subGraph = new SubGraph
        {
            Nodes = new[] { node1, node2 },
            Edges = new[] { edge1 }
        };

        var generator = new SubGraphDeterministicTextGenerator(DeterministicTextFormat.Markdown);

        // Act
        var result1 = await generator.GenerateAsync(subGraph);
        var result2 = await generator.GenerateAsync(subGraph);

        // Assert
        Assert.AreEqual(result1, result2, "Les générations doivent être identiques (déterministes)");
        Assert.IsTrue(result1.Contains("# Sous-graphe"));
        Assert.IsTrue(result1.Contains("## Nœuds"));
        Assert.IsTrue(result1.Contains("## Arêtes"));

        // Vérifier que Machine Learning (ID plus petit) apparaît avant Deep Learning
        var mlIndex = result1.IndexOf("Machine Learning");
        var dlIndex = result1.IndexOf("Deep Learning");
        Assert.IsTrue(mlIndex < dlIndex, "Les nœuds doivent être triés par ID");
    }

    [TestMethod]
    public async Task SubGraphDeterministicTextGenerator_Yaml_ShouldGenerateStableOutputWithSortedElements()
    {
        // Arrange
        var node1 = new GraphNode(
            Id: Guid.Parse("00000000-0000-0000-0000-000000000003"),
            Type: "Concept",
            Label: "Deep Learning",
            Description: "Réseaux de neurones profonds",
            Properties: new Dictionary<string, object?>
            {
                { "Field", "Artificial Intelligence" },
                { "Techniques", new[] { "Neural Networks", "Deep Neural Networks" } }
            });

        var node2 = new GraphNode(
            Id: Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Type: "Concept",
            Label: "Machine Learning",
            Description: "Apprentissage automatique",
            Properties: new Dictionary<string, object?>
            {
                { "Field", "Artificial Intelligence" },
                { "Techniques", new[] { "Supervised Learning", "Unsupervised Learning", "Reinforcement Learning" } }
            });

        var edge1 = new GraphEdge(
            Id: Guid.Parse("00000000-0000-0000-0000-000000000005"),
            SourceId: node2.Id,
            TargetId: node1.Id,
            Type: "INCLUDES",
            Properties: new Dictionary<string, object?>
            {
                { "Description", "Le machine learning inclut le deep learning" }
            });

        var subGraph = new SubGraph
        {
            Nodes = new[] { node1, node2 },
            Edges = new[] { edge1 }
        };

        var generator = new SubGraphDeterministicTextGenerator(DeterministicTextFormat.Yaml);

        // Act
        var result1 = await generator.GenerateAsync(subGraph);
        var result2 = await generator.GenerateAsync(subGraph);

        // Assert
        Assert.AreEqual(result1, result2, "Les générations doivent être identiques (déterministes)");
        Assert.IsTrue(result1.Contains("subgraph:"));
        Assert.IsTrue(result1.Contains("nodes:"));
        Assert.IsTrue(result1.Contains("edges:"));
    }

    [TestMethod]
    public async Task SubGraphDeterministicTextGenerator_EmptySubGraph_ShouldHandleGracefully()
    {
        // Arrange
        var subGraph = new SubGraph
        {
            Nodes = Array.Empty<GraphNode>(),
            Edges = Array.Empty<GraphEdge>()
        };

        var markdownGenerator = new SubGraphDeterministicTextGenerator(DeterministicTextFormat.Markdown);
        var yamlGenerator = new SubGraphDeterministicTextGenerator(DeterministicTextFormat.Yaml);

        // Act
        var markdownResult = await markdownGenerator.GenerateAsync(subGraph);
        var yamlResult = await yamlGenerator.GenerateAsync(subGraph);

        // Assert
        Assert.IsTrue(markdownResult.Contains("*Aucun nœud*"));
        Assert.IsTrue(markdownResult.Contains("*Aucune arête*"));
        Assert.IsTrue(yamlResult.Contains("nodes:"));
        Assert.IsTrue(yamlResult.Contains("edges:"));
    }
}
