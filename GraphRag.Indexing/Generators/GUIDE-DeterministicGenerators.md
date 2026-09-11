# Guide d'utilisation - Générateurs déterministes

## Introduction

Ce guide présente des scénarios d'utilisation pratiques des générateurs de texte déterministes pour les graphes de connaissances.

## Scénario 1 : Pipeline d'indexation vectorielle stable

```csharp
public class VectorIndexingPipeline
{
	private readonly NodeDeterministicTextGenerator _textGenerator;
	private readonly IEmbeddingService _embeddingService;

	public VectorIndexingPipeline(IEmbeddingService embeddingService)
	{
		// Utiliser le format Markdown pour plus de contexte
		_textGenerator = new NodeDeterministicTextGenerator(DeterministicTextFormat.Markdown);
		_embeddingService = embeddingService;
	}

	public async Task<VectorRecord> IndexNodeAsync(GraphNode node)
	{
		// Génération déterministe du texte
		var text = await _textGenerator.GenerateAsync(node);

		// Création de l'embedding
		var embedding = await _embeddingService.GenerateEmbeddingAsync(text);

		return new VectorRecord
		{
			Id = node.Id,
			Text = text,
			Embedding = embedding,
			Metadata = new Dictionary<string, object>
			{
				["type"] = node.Type,
				["label"] = node.Label
			}
		};
	}
}
```

**Avantages :**
- ✅ Même nœud = même texte = même embedding (réindexation idempotente)
- ✅ Pas de variation due au LLM (température = 0 de facto)
- ✅ Coût zéro pour la génération de texte

## Scénario 2 : Export et synchronisation entre systèmes

```csharp
public class GraphExportService
{
	private readonly SubGraphDeterministicTextGenerator _yamlGenerator;

	public GraphExportService()
	{
		_yamlGenerator = new SubGraphDeterministicTextGenerator(DeterministicTextFormat.Yaml);
	}

	public async Task ExportToFileAsync(SubGraph graph, string outputPath)
	{
		var yaml = await _yamlGenerator.GenerateAsync(graph);
		await File.WriteAllTextAsync(outputPath, yaml);
	}

	public async Task<bool> HasGraphChangedAsync(SubGraph current, string previousExportPath)
	{
		var currentYaml = await _yamlGenerator.GenerateAsync(current);

		if (!File.Exists(previousExportPath))
			return true;

		var previousYaml = await File.ReadAllTextAsync(previousExportPath);

		// Comparaison déterministe : si le contenu est identique, le graphe n'a pas changé
		return currentYaml != previousYaml;
	}
}
```

**Avantages :**
- ✅ Détection fiable des changements via simple comparaison de texte
- ✅ Format YAML parsable par d'autres outils
- ✅ Versioning simplifié (Git, etc.)

## Scénario 3 : Génération de documentation automatique

```csharp
public class DocumentationGenerator
{
	private readonly SubGraphDeterministicTextGenerator _markdownGenerator;

	public DocumentationGenerator()
	{
		_markdownGenerator = new SubGraphDeterministicTextGenerator(DeterministicTextFormat.Markdown);
	}

	public async Task GenerateDocumentationAsync(
		string projectName,
		SubGraph knowledgeGraph,
		string outputDirectory)
	{
		var markdown = await _markdownGenerator.GenerateAsync(knowledgeGraph);

		// Créer un document complet
		var fullDocument = $@"# Documentation du projet {projectName}

*Document généré automatiquement le {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC*

{markdown}

---

*Cette documentation est générée à partir du graphe de connaissances du projet.*
";

		var outputPath = Path.Combine(outputDirectory, $"{projectName}-knowledge-graph.md");
		await File.WriteAllTextAsync(outputPath, fullDocument);
	}
}
```

**Avantages :**
- ✅ Documentation toujours à jour avec le graphe
- ✅ Format Markdown lisible et versionnable
- ✅ Génération rapide et prédictible

## Scénario 4 : Mode hybride (déterministe + IA)

```csharp
public class HybridTextGenerator
{
	private readonly NodeDeterministicTextGenerator _deterministicGenerator;
	private readonly NodeTextGenerator _aiGenerator;
	private readonly bool _useAI;

	public HybridTextGenerator(
		IChatClient chatClient,
		bool useAI = false)
	{
		_deterministicGenerator = new NodeDeterministicTextGenerator(DeterministicTextFormat.Markdown);
		_aiGenerator = new NodeTextGenerator(chatClient);
		_useAI = useAI;
	}

	public async Task<string> GenerateAsync(GraphNode node, CancellationToken ct = default)
	{
		if (_useAI)
		{
			try
			{
				// Tenter la génération enrichie par IA
				return await _aiGenerator.GenerateAsync(node, ct);
			}
			catch (Exception ex)
			{
				// En cas d'erreur (quota dépassé, timeout, etc.)
				// Fallback vers le générateur déterministe
				Console.WriteLine($"AI generation failed, using deterministic fallback: {ex.Message}");
				return await _deterministicGenerator.GenerateAsync(node, ct);
			}
		}

		// Mode déterministe par défaut
		return await _deterministicGenerator.GenerateAsync(node, ct);
	}
}
```

**Avantages :**
- ✅ Résilience : fallback automatique en cas de problème avec l'IA
- ✅ Flexibilité : mode configurable selon l'environnement
- ✅ Optimisation des coûts : IA uniquement quand nécessaire

## Scénario 5 : Batch processing avec garantie de reproductibilité

```csharp
public class BatchGraphProcessor
{
	private readonly EdgeDeterministicTextGenerator _edgeGenerator;
	private readonly NodeDeterministicTextGenerator _nodeGenerator;

	public BatchGraphProcessor()
	{
		_edgeGenerator = new EdgeDeterministicTextGenerator(DeterministicTextFormat.Yaml);
		_nodeGenerator = new NodeDeterministicTextGenerator(DeterministicTextFormat.Yaml);
	}

	public async Task<ProcessingReport> ProcessGraphBatchAsync(
		IEnumerable<GraphNode> nodes,
		IEnumerable<GraphEdge> edges)
	{
		var report = new ProcessingReport();
		var processedNodes = new List<string>();
		var processedEdges = new List<string>();

		// Tri déterministe pour garantir le même ordre de traitement
		var sortedNodes = nodes.OrderBy(n => n.Id).ToList();
		var sortedEdges = edges.OrderBy(e => e.Id).ToList();

		foreach (var node in sortedNodes)
		{
			var yaml = await _nodeGenerator.GenerateAsync(node);
			processedNodes.Add(yaml);
			report.NodesProcessed++;
		}

		foreach (var edge in sortedEdges)
		{
			var yaml = await _edgeGenerator.GenerateAsync(edge);
			processedEdges.Add(yaml);
			report.EdgesProcessed++;
		}

		// Génération d'un hash déterministe du batch
		var combined = string.Join("\n---\n", processedNodes.Concat(processedEdges));
		report.BatchHash = ComputeSha256Hash(combined);

		return report;
	}

	private static string ComputeSha256Hash(string text)
	{
		using var sha256 = System.Security.Cryptography.SHA256.Create();
		var bytes = System.Text.Encoding.UTF8.GetBytes(text);
		var hash = sha256.ComputeHash(bytes);
		return Convert.ToHexString(hash);
	}
}

public class ProcessingReport
{
	public int NodesProcessed { get; set; }
	public int EdgesProcessed { get; set; }
	public string BatchHash { get; set; } = "";
}
```

**Avantages :**
- ✅ Hash déterministe pour vérifier l'intégrité du traitement
- ✅ Même batch = même hash (reproductibilité garantie)
- ✅ Audit trail fiable

## Scénario 6 : Testing et validation

```csharp
[TestClass]
public class GraphValidationTests
{
	[TestMethod]
	public async Task GraphShouldBeStableAcrossRuns()
	{
		// Arrange
		var graph = CreateSampleGraph();
		var generator = new SubGraphDeterministicTextGenerator(DeterministicTextFormat.Yaml);

		// Act - Générer 10 fois
		var results = new List<string>();
		for (int i = 0; i < 10; i++)
		{
			results.Add(await generator.GenerateAsync(graph));
		}

		// Assert - Toutes les générations doivent être identiques
		var firstResult = results[0];
		Assert.IsTrue(results.All(r => r == firstResult), 
			"Le graphe doit produire exactement la même sortie à chaque génération");
	}

	[TestMethod]
	public async Task MinimalGraphChangeShouldBeDetectable()
	{
		// Arrange
		var graph1 = CreateSampleGraph();
		var graph2 = CreateSampleGraph();

		// Modifier un seul caractère dans la description d'un nœud
		var modifiedNode = graph2.Nodes.First() with 
		{ 
			Description = graph2.Nodes.First().Description + "." 
		};

		graph2 = graph2 with 
		{ 
			Nodes = new[] { modifiedNode }.Concat(graph2.Nodes.Skip(1)).ToArray() 
		};

		var generator = new SubGraphDeterministicTextGenerator(DeterministicTextFormat.Yaml);

		// Act
		var yaml1 = await generator.GenerateAsync(graph1);
		var yaml2 = await generator.GenerateAsync(graph2);

		// Assert
		Assert.AreNotEqual(yaml1, yaml2, 
			"Même un changement minimal doit être détectable");
	}

	private static SubGraph CreateSampleGraph()
	{
		var node = new GraphNode(
			Guid.Parse("12345678-1234-1234-1234-123456789abc"),
			"Test",
			"Sample",
			"Description"
		);

		return new SubGraph
		{
			Nodes = new[] { node },
			Edges = Array.Empty<GraphEdge>()
		};
	}
}
```

**Avantages :**
- ✅ Tests fiables et reproductibles
- ✅ Validation de l'intégrité des données
- ✅ Détection précise des changements

## Comparaison des formats

| Aspect | Markdown | YAML |
|--------|----------|------|
| **Lisibilité humaine** | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ |
| **Parsing automatique** | ⭐⭐ | ⭐⭐⭐⭐⭐ |
| **Taille** | Moyenne | Compact |
| **Richesse visuelle** | Élevée | Basique |
| **Cas d'usage** | Documentation, présentation | Export, intégration, API |

## Recommandations par cas d'usage

| Cas d'usage | Format recommandé | Générateur |
|-------------|------------------|------------|
| Indexation vectorielle | Markdown | Node/Edge/SubGraph |
| Export vers autre système | YAML | SubGraph |
| Documentation projet | Markdown | SubGraph |
| Tests unitaires | YAML ou Markdown | Node/Edge |
| CI/CD validation | YAML | SubGraph |
| Visualisation humaine | Markdown | SubGraph |

## Performance

Les générateurs déterministes sont extrêmement rapides car ils n'appellent aucune API externe :

```csharp
// Benchmark typique :
// - Génération nœud : < 1 ms
// - Génération arête : < 1 ms  
// - Génération sous-graphe (100 nœuds, 200 arêtes) : < 50 ms
```

Comparé aux générateurs IA :
- 🚀 **1000x plus rapide** (pas d'appel réseau)
- 💰 **Coût zéro** (pas d'API payante)
- 🎯 **100% déterministe** (pas de variabilité)

## Conclusion

Les générateurs déterministes sont essentiels pour :
- ✅ Pipelines de production stables
- ✅ Tests reproductibles
- ✅ Audit et compliance
- ✅ Optimisation des coûts

Utilisez-les comme base fiable, et ajoutez les générateurs IA uniquement quand la richesse sémantique supplémentaire justifie le coût et la variabilité.
