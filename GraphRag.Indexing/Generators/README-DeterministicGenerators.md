# Générateurs de Texte Déterministes

Ce dossier contient les générateurs de texte déterministes pour les graphes de connaissances.

## Vue d'ensemble

Les générateurs déterministes produisent des projections structurées et stables des éléments du graphe (nœuds, arêtes, sous-graphes) sans utiliser de modèle de langage (LLM). Contrairement aux générateurs basés sur l'IA comme `NodeTextGenerator`, ces générateurs garantissent:

- ✅ **Déterminisme complet** : la même entrée produit toujours la même sortie
- ✅ **Ordre stable** : les éléments sont triés de manière déterministe (par ID)
- ✅ **Pas d'hallucinations** : aucune information n'est inventée
- ✅ **Performance** : génération instantanée sans appel API
- ✅ **Formats structurés** : Markdown ou YAML

## Générateurs disponibles

### 1. NodeDeterministicTextGenerator

Génère une représentation structurée d'un nœud de graphe.

**Exemple d'utilisation :**

```csharp
var node = new GraphNode(
	Id: Guid.NewGuid(),
	Type: "Concept",
	Label: "Machine Learning",
	Description: "Un domaine de l'intelligence artificielle"
);

// Format Markdown
var markdownGenerator = new NodeDeterministicTextGenerator(DeterministicTextFormat.Markdown);
var markdown = await markdownGenerator.GenerateAsync(node);

// Format YAML
var yamlGenerator = new NodeDeterministicTextGenerator(DeterministicTextFormat.Yaml);
var yaml = await yamlGenerator.GenerateAsync(node);
```

**Sortie Markdown :**
```markdown
# Machine Learning

## Informations

- **ID**: `12345678-1234-1234-1234-123456789abc`
- **Type**: Concept
- **Nom**: Machine Learning

## Description

Un domaine de l'intelligence artificielle
```

**Sortie YAML :**
```yaml
node:
  id: "12345678-1234-1234-1234-123456789abc"
  type: "Concept"
  label: "Machine Learning"
  description: "Un domaine de l'intelligence artificielle"
```

### 2. EdgeDeterministicTextGenerator

Génère une représentation structurée d'une arête de graphe.

**Exemple d'utilisation :**

```csharp
var edge = new GraphEdge(
	Id: Guid.NewGuid(),
	SourceId: sourceNodeId,
	TargetId: targetNodeId,
	Type: "USES"
);

var generator = new EdgeDeterministicTextGenerator(DeterministicTextFormat.Markdown);
var result = await generator.GenerateAsync(edge);
```

**Sortie Markdown :**
```markdown
# Arête: USES

## Informations

- **ID**: `12345678-1234-1234-1234-123456789abc`
- **Type**: USES
- **Source**: `11111111-1111-1111-1111-111111111111`
- **Cible**: `22222222-2222-2222-2222-222222222222`
```

### 3. SubGraphDeterministicTextGenerator

Génère une représentation structurée d'un sous-graphe complet.

**Exemple d'utilisation :**

```csharp
var subGraph = new SubGraph
{
	Nodes = new[] { node1, node2, node3 },
	Edges = new[] { edge1, edge2 }
};

var generator = new SubGraphDeterministicTextGenerator(DeterministicTextFormat.Markdown);
var result = await generator.GenerateAsync(subGraph);
```

**Sortie Markdown :**
```markdown
# Sous-graphe

## Nœuds

### Machine Learning

- **ID**: `11111111-1111-1111-1111-111111111111`
- **Type**: Concept
- **Description**: Apprentissage automatique

### Deep Learning

- **ID**: `22222222-2222-2222-2222-222222222222`
- **Type**: Concept
- **Description**: Réseaux de neurones profonds

## Arêtes

- `11111111-1111-1111-1111-111111111111` → `22222222-2222-2222-2222-222222222222` (INCLUDES)
```

## Format de sortie

Tous les générateurs supportent deux formats :

- **Markdown** : format lisible pour la documentation et la présentation
- **YAML** : format structuré pour le parsing automatique et l'intégration

## Caractéristiques techniques

### Tri déterministe

Les collections (nœuds et arêtes dans un sous-graphe) sont toujours triées par ID pour garantir un ordre stable :

```csharp
var sortedNodes = subGraph.Nodes.OrderBy(n => n.Id).ToList();
var sortedEdges = subGraph.Edges.OrderBy(e => e.Id).ToList();
```

### Échappement de caractères

Les générateurs gèrent correctement l'échappement des caractères spéciaux :

**Markdown :**
- `\`, `` ` ``, `*`, `_`, `[`, `]`

**YAML :**
- `\`, `"`, `\n`, `\r`

### Performance

Les générateurs sont synchrones en interne (pas d'appel I/O) et retournent immédiatement via `Task.FromResult()`.

## Cas d'usage

### 1. Indexation vectorielle stable

Utilisez les générateurs déterministes pour créer des embeddings cohérents :

```csharp
var generator = new NodeDeterministicTextGenerator(DeterministicTextFormat.Markdown);
var text = await generator.GenerateAsync(node);
var embedding = await embeddingService.GenerateEmbeddingAsync(text);
```

### 2. Documentation automatique

Générez de la documentation structurée à partir de votre graphe :

```csharp
var generator = new SubGraphDeterministicTextGenerator(DeterministicTextFormat.Markdown);
var documentation = await generator.GenerateAsync(knowledgeSubGraph);
await File.WriteAllTextAsync("knowledge-graph.md", documentation);
```

### 3. Export de données

Exportez vos graphes dans un format structuré pour l'intégration avec d'autres systèmes :

```csharp
var generator = new SubGraphDeterministicTextGenerator(DeterministicTextFormat.Yaml);
var yaml = await generator.GenerateAsync(subGraph);
await File.WriteAllTextAsync("graph-export.yml", yaml);
```

## Comparaison avec les générateurs IA

| Caractéristique | Générateurs déterministes | Générateurs IA |
|----------------|---------------------------|----------------|
| Déterminisme | ✅ Garanti | ❌ Variable |
| Performance | ✅ Instantané | ⚠️ Dépend du LLM |
| Coût | ✅ Gratuit | 💰 Coût par appel |
| Richesse | ⚠️ Basique | ✅ Enrichi |
| Hallucinations | ✅ Impossible | ⚠️ Possible |
| Stabilité | ✅ 100% | ⚠️ Variable |

## Recommandations

- **Utilisez les générateurs déterministes** pour :
  - L'indexation vectorielle
  - Les exports de données
  - Les environnements sans accès LLM
  - Les besoins de reproductibilité stricte

- **Utilisez les générateurs IA** pour :
  - La génération de descriptions enrichies
  - La recherche sémantique avancée
  - Les besoins de synthèse intelligente

## Tests

Des tests unitaires complets sont disponibles dans `DeterministicTextGeneratorTests.cs` pour valider :
- Le déterminisme (même entrée → même sortie)
- Le tri stable des collections
- La gestion des cas limites (graphes vides)
- L'échappement des caractères spéciaux
