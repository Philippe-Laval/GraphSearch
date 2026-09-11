# Modularity Calculator

Ce module fournit des fonctionnalités pour calculer les scores de modularité pour les graphes de connaissances et leurs communautés.

## Qu'est-ce que la modularité ?

La **modularité** mesure la force avec laquelle un graphe est divisé en communautés.

Une communauté a une modularité élevée lorsque :
- De nombreuses arêtes connectent des nœuds **à l'intérieur** de la communauté
- Relativement peu d'arêtes connectent ces nœuds au **reste du graphe**

## Formules

### Modularité du graphe complet

Pour une partition complète d'un graphe non orienté :

```
Q = (1/2m) * Σ(A_ij - k_i*k_j/(2m)) * δ(c_i, c_j)
```

Où :
- `A_ij` : 1 si les nœuds i et j sont connectés, 0 sinon
- `k_i`, `k_j` : degrés des nœuds i et j
- `m` : nombre total d'arêtes
- `δ(c_i, c_j)` : 1 si les nœuds sont dans la même communauté, 0 sinon

### Modularité d'une communauté

La contribution d'une communauté individuelle peut être calculée comme :

```
Q_C = l_C/m - (d_C/(2m))²
```

Où :
- `l_C` : nombre d'arêtes dans la communauté C
- `d_C` : somme des degrés des nœuds de la communauté
- `m` : nombre total d'arêtes dans le graphe

## Interprétation

- **Q > 0 (positif élevé)** : structure communautaire forte
- **Q ≈ 0** : connectivité similaire à un graphe aléatoire
- **Q < 0 (négatif)** : moins de connexions internes qu'attendu

## Utilisation

### Calcul de la modularité du graphe complet

```csharp
using Library.ExtractionForGraphKnowledge.Communities;
using Library.ExtractionForGraphKnowledge.Models;

// Avec la classe statique
var communities = new List<GraphCommunity> { /* vos communautés */ };
float overallModularity = ModularityCalculator.CalculateGraphModularity(communities);

Console.WriteLine($"Modularité globale : {overallModularity:F4}");

// Avec les méthodes d'extension
float modularity = communities.CalculateOverallModularity();
```

### Calcul de la modularité d'une communauté spécifique

```csharp
using Library.ExtractionForGraphKnowledge.Communities;
using Library.ExtractionForGraphKnowledge.Models;

var community = /* votre communauté */;
int totalEdgesInGraph = /* nombre total d'arêtes */;

// Avec la classe statique
float communityModularity = ModularityCalculator.CalculateCommunityModularity(
	community, 
	totalEdgesInGraph);

Console.WriteLine($"Contribution de la communauté : {communityModularity:F4}");

// Avec les méthodes d'extension
float modularityContribution = community.CalculateModularityContribution(totalEdgesInGraph);
```

### Exemple complet

```csharp
using Library.ExtractionForGraphKnowledge.Communities;
using Library.ExtractionForGraphKnowledge.Models;

// Supposons que vous avez détecté des communautés
var communities = new List<GraphCommunity>
{
	new GraphCommunity
	{
		Id = Guid.NewGuid(),
		Nodes = nodes1,
		Edges = edges1,
		Level = 0,
		Modularity = 0f // sera recalculé
	},
	new GraphCommunity
	{
		Id = Guid.NewGuid(),
		Nodes = nodes2,
		Edges = edges2,
		Level = 0,
		Modularity = 0f
	}
};

// Calculer la modularité globale
float globalModularity = communities.CalculateOverallModularity();
Console.WriteLine($"Modularité globale du graphe : {globalModularity:F4}");

// Calculer le nombre total d'arêtes
int totalEdges = communities
	.SelectMany(c => c.Edges)
	.Distinct()
	.Count();

// Calculer la contribution de chaque communauté
foreach (var community in communities)
{
	float contribution = community.CalculateModularityContribution(totalEdges);
	Console.WriteLine($"Communauté {community.Id}: Contribution = {contribution:F4}");
}
```

## Notes importantes

1. **Graphes non orientés** : Les calculs supposent un graphe non orienté. Chaque arête contribue au degré des deux nœuds.

2. **Performance** : Pour les grands graphes, le calcul de la modularité globale peut être coûteux (O(n²) où n est le nombre de nœuds).

3. **Validation** : Les scores de modularité sont généralement compris entre -0.5 et 1.0, bien que théoriquement ils puissent être en dehors de cette plage.

4. **Utilisation avec détection de communautés** : Ces calculs sont souvent utilisés pour :
   - Évaluer la qualité de la détection de communautés
   - Comparer différents algorithmes de partitionnement
   - Optimiser les paramètres de détection de communautés
