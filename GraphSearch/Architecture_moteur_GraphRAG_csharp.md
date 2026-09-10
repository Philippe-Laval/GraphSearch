
# Architecture moteur GraphRAG en C#

Je programme en C#. Tu m'avais dit : 

```
Cypher 
│ 
▼ 
ANTLR AST 
│ 
▼ 
Binder 
│ 
▼ 
Logical Plan 
│ 
▼ 
Optimizer 
│ 
▼ 
Physical Plan 
│ 
▼ 
Execution Engine
```

Oui. C'est l'architecture utilisée (avec des variantes) par la plupart des moteurs de bases de données relationnelles et de graphes comme Neo4j, SQL Server, PostgreSQL, DuckDB, Apache Calcite, etc.

Pour un moteur GraphRAG écrit en C#, je structurerais même un peu plus les étapes.

```text
                 +----------------+
Cypher Query --->| Lexer (ANTLR)  |
                 +----------------+
                         │
                         ▼
                 +----------------+
                 | Parser (ANTLR) |
                 +----------------+
                         │
                         ▼
                   Syntax AST
                         │
                         ▼
                 +----------------+
                 |    Binder      |
                 +----------------+
                         │
                         ▼
                  Bound AST
                         │
                         ▼
                 +----------------+
                 | Logical Planner|
                 +----------------+
                         │
                         ▼
                  Logical Plan
                         │
                         ▼
                 +----------------+
                 |   Optimizer    |
                 +----------------+
                         │
                         ▼
                Optimized Logical Plan
                         │
                         ▼
                 +----------------+
                 | Physical Planner|
                 +----------------+
                         │
                         ▼
                  Physical Plan
                         │
                         ▼
                 +----------------+
                 | Execution Engine|
                 +----------------+
                         │
                         ▼
                      Results
```

Chaque étape a une responsabilité très précise.

---

# 1. Parser (ANTLR)

Il transforme le texte en AST.

Entrée

```cypher
MATCH (p:Person)-[:KNOWS]->(f)
WHERE p.age > 30
RETURN f.name
```

Sortie

```
MatchClause
    Pattern
        Node(p)
        Relationship(KNOWS)
        Node(f)

WhereClause
    GreaterThan
        Property(p.age)
        Constant(30)

ReturnClause
    Property(f.name)
```

Cet AST est uniquement syntaxique.

Il ne connaît rien du graphe.

---

# 2. Binder

Le Binder est extrêmement important.

Il résout les noms.

Par exemple

```cypher
MATCH (p:Person)
RETURN p.name
```

Il vérifie :

* p existe
* Person est un label valide
* name est une propriété connue (optionnel)
* le type de p

Il remplace les noms par des références.

Avant :

```
PropertyExpression
    Variable("p")
    "name"
```

Après :

```
BoundPropertyExpression
    VariableId = 12
    PropertyId = 4
```

Il peut également :

* inférer les types
* gérer les scopes
* détecter les erreurs

---

# 3. Logical Planner

Le Binder produit encore un arbre d'expressions.

Le Logical Planner produit un arbre d'opérateurs.

Par exemple

```cypher
MATCH (p:Person)-[:KNOWS]->(f)
WHERE p.age > 30
RETURN f
```

devient

```
Projection
    Expand
        Filter
            NodeScan(Person)
```

Autrement dit

```
NodeScan(Person)

↓

Filter(age>30)

↓

Expand(KNOWS)

↓

Projection(f)
```

Ici il n'y a aucune notion de stockage.

---

# 4. Optimizer

L'Optimizer transforme le Logical Plan en un meilleur Logical Plan.

Exemple.

Au lieu de

```
Expand

↓

Filter
```

on préfère

```
Filter

↓

Expand
```

pour réduire le nombre de relations parcourues.

Autres optimisations :

* Push Down Predicate
* Projection Pushdown
* Join Reordering
* Constant Folding
* Elimination des opérateurs inutiles
* Utilisation d'index
* Réécriture des expressions

Exemple :

```
Filter(age>30)

↓

NodeScan(Person)
```

devient

```
NodeIndexSeek(Person.age)
```

---

# 5. Physical Planner

Le Logical Plan dit :

```
NodeScan(Person)
```

Le Physical Planner décide comment le réaliser.

Par exemple

```
NodeIndexSeek
```

ou

```
ParallelNodeScan
```

ou

```
MemoryNodeScan
```

ou

```
DiskNodeScan
```

Le plan logique ne dépend jamais du stockage.

Le plan physique, si.

---

# 6. Execution Engine

Il exécute le plan.

Chaque opérateur produit des lignes.

```
NodeIndexSeek

↓

Expand

↓

Projection
```

L'exécution ressemble beaucoup à LINQ.

Chaque opérateur est un itérateur.

```csharp
foreach(var row in child.Execute())
{
    ...
    yield return newRow;
}
```

C'est exactement comme PostgreSQL.

---

# Ce que je ferais pour un moteur GraphRAG

Je découperais encore davantage.

```
GraphQueryEngine
│
├── Parsing
│      Lexer
│      Parser
│      AstBuilder
│
├── Binding
│      SymbolTable
│      Scope
│      TypeResolver
│      Binder
│
├── LogicalPlanning
│      LogicalPlanBuilder
│      Operators
│
├── Optimization
│      RuleEngine
│      Rules
│
├── PhysicalPlanning
│      PhysicalPlanBuilder
│      CostEstimator
│
├── Execution
│      Pipelines
│      Operators
│      Runtime
│
├── Storage
│      NodeStore
│      EdgeStore
│      PropertyStore
│      Indexes
│
└── VectorSearch
       EmbeddingRetriever
       BM25Retriever
       HybridRetriever
       Reranker
```

---

# Avec GraphRAG, j'ajouterais une étape supplémentaire

Pour un moteur GraphRAG moderne, le pipeline devient souvent :

```
Cypher

↓

ANTLR

↓

Binder

↓

Logical Plan

↓

Optimizer

↓

Physical Plan

↓

Execution

↓

Graph Result
       │
       ▼
Hybrid Retriever
       │
       ▼
Context Builder
       │
       ▼
LLM
```

Le moteur de requête retourne un sous-graphe (nœuds, relations, propriétés). Ensuite, un **Hybrid Retriever** peut compléter ce résultat avec des chunks textuels issus d'un index vectoriel (embeddings) et/ou d'un index BM25. Un **Context Builder** fusionne ces informations, élimine les doublons, ordonne les éléments par pertinence et construit le contexte envoyé au LLM. Cette séparation est très intéressante, car le moteur de requête reste indépendant du modèle de langage et peut être utilisé seul, tandis que la couche GraphRAG enrichit les résultats lorsqu'une génération de réponse est nécessaire.

Pour le moteur que vous développez en C#/.NET, cette architecture modulaire permet aussi de remplacer facilement le stockage, le moteur vectoriel ou le fournisseur de LLM sans modifier le compilateur Cypher lui-même.

# Peux tu m'aider à créer le Binder en C# ?

Oui. D'ailleurs, je considère que **le Binder est la pièce la plus importante du moteur**, plus encore que l'optimizer. C'est lui qui transforme un AST purement syntaxique en un arbre sémantique exploitable par le reste du pipeline.

Pour un moteur GraphRAG, je ne copierais pas exactement Neo4j. Je profiterais de C# pour construire quelque chose de fortement typé et facilement extensible.

# Les responsabilités du Binder

Le Binder doit effectuer plusieurs tâches :

```
AST
 │
 ▼
Créer les scopes
 │
 ▼
Déclarer les variables
 │
 ▼
Résoudre les références
 │
 ▼
Résoudre les labels
 │
 ▼
Résoudre les types
 │
 ▼
Créer un Bound AST
 │
 ▼
Détecter les erreurs
```

Par exemple

```cypher
MATCH (p:Person)-[:KNOWS]->(f)
WHERE p.age > 30
RETURN f.name
```

L'AST contient uniquement des chaînes :

```
Variable = "p"
Label = "Person"
Property = "age"
```

Le Binder doit produire :

```
VariableId = 1
NodeType = Person
PropertyId = 42
PropertyType = Integer
```

Il n'y a plus de chaînes de caractères à résoudre ensuite.

---

# Architecture

Je découperais le Binder en plusieurs classes.

```
Binder
│
├── Scope
├── SymbolTable
├── BinderContext
├── ExpressionBinder
├── PatternBinder
├── ClauseBinder
├── TypeResolver
├── SemanticModel
└── Diagnostics
```

Le Binder lui-même reste très petit.

```csharp
public sealed class Binder
{
    public BoundQuery Bind(QueryAst query)
    {
        var context = new BinderContext();

        return new QueryBinder(context).Bind(query);
    }
}
```

---

# BinderContext

Le contexte contient tout l'état.

```csharp
public sealed class BinderContext
{
    public Scope CurrentScope { get; set; }

    public SymbolTable Symbols { get; }

    public GraphSchema Schema { get; }

    public DiagnosticBag Diagnostics { get; }

    public TypeResolver Types { get; }

    public BinderContext(
        GraphSchema schema)
    {
        Schema = schema;
        Symbols = new SymbolTable();
        Diagnostics = new DiagnosticBag();
        Types = new TypeResolver(schema);

        CurrentScope = Scope.Root;
    }
}
```

Il est passé à tous les binders.

---

# Scope

Chaque clause peut créer un scope.

```
MATCH

RETURN

WITH

CALL

EXISTS

```

Le scope ressemble beaucoup à celui d'un compilateur C#.

```csharp
public sealed class Scope
{
    public Scope? Parent { get; }

    Dictionary<string, Symbol> _symbols;

    public Scope(Scope? parent)
    {
        Parent = parent;
    }

    public bool TryLookup(
        string name,
        out Symbol symbol)
    {
        ...
    }

    public void Declare(Symbol symbol)
    {
        ...
    }
}
```

---

# Symbol

Toutes les variables deviennent des Symbol.

```
MATCH (p)

MATCH (movie)

RETURN p
```

devient

```
VariableSymbol

Name = p

Type = Node

Id = 1
```

```csharp
public abstract class Symbol
{
    public string Name { get; }

    protected Symbol(string name)
    {
        Name = name;
    }
}
```

Puis

```csharp
public sealed class VariableSymbol : Symbol
{
    public GraphType Type { get; }

    public int Id { get; }

    public VariableSymbol(
        int id,
        string name,
        GraphType type)
        : base(name)
    {
        Id = id;
        Type = type;
    }
}
```

---

# GraphType

Le Binder doit manipuler des types.

Je créerais une hiérarchie.

```csharp
abstract class GraphType
```

Puis

```
NodeType

RelationshipType

PathType

ListType

BooleanType

IntegerType

FloatType

StringType

NullType

AnyType
```

On évite complètement les chaînes.

---

# TypeResolver

Le TypeResolver connaît le schéma.

```
Person.age -> Integer

Movie.title -> String

Movie.rating -> Float
```

API

```csharp
GraphType ResolveProperty(
    NodeType node,
    string property)
```

Le Binder ne connaît jamais le schéma directement.

---

# ExpressionBinder

C'est probablement la plus grosse classe.

Elle transforme

```
PropertyExpression
```

en

```
BoundPropertyExpression
```

Avant

```
p.age
```

Après

```
BoundPropertyExpression

VariableId = 4

PropertyId = 12

Type = Integer
```

---

# PatternBinder

Le PatternBinder construit tous les symboles.

Exemple

```
(p)

```

↓

```
VariableSymbol
```

Puis

```
(p)-[:KNOWS]->(f)
```

↓

```
Variable p

Variable f

Relationship KNOWS
```

---

# ClauseBinder

Chaque clause est indépendante.

```
MatchBinder

WhereBinder

ReturnBinder

WithBinder

CreateBinder

MergeBinder

DeleteBinder
```

Le Binder principal fait simplement :

```csharp
foreach(var clause in query.Clauses)
{
    BindClause(clause);
}
```

---

# Bound AST

À partir de cette étape, l'AST d'origine n'est plus utilisé.

On travaille sur un arbre fortement typé :

```
BoundQuery

BoundMatchClause

BoundReturnClause

BoundBinaryExpression

BoundVariableExpression

BoundPropertyExpression

BoundLiteralExpression
```

Par exemple :

```csharp
abstract class BoundExpression
{
    public GraphType Type { get; }

    protected BoundExpression(GraphType type)
    {
        Type = type;
    }
}
```

Puis :

```csharp
sealed class BoundPropertyExpression
    : BoundExpression
{
    public VariableSymbol Variable { get; }

    public GraphProperty Property { get; }

    ...
}
```

On remarque que l'expression contient directement les objets `VariableSymbol` et `GraphProperty`, sans nouvelle recherche par nom.

---

# Diagnostics

Le Binder ne devrait jamais lever une exception pour une erreur de requête. Il accumule les diagnostics :

```
Unknown variable p

Unknown label Person

Unknown property age

Type mismatch

Cannot compare String with Integer

Duplicate variable p

Variable not in scope
```

Le pipeline décide ensuite s'il continue (pour afficher plusieurs erreurs d'un coup) ou s'arrête.

---

# Séparation en visiteurs

Pour éviter une classe monolithique de plusieurs milliers de lignes, j'utiliserais des visiteurs spécialisés :

```
Binder
│
├── MatchClauseBinder
├── CreateClauseBinder
├── MergeClauseBinder
├── ReturnClauseBinder
├── WithClauseBinder
├── UnwindClauseBinder
├── DeleteClauseBinder
│
├── PatternBinder
├── ExpressionBinder
├── TypeResolver
└── ScopeManager
```

Chaque classe reste concentrée sur une responsabilité unique.

## Une approche qui s'intègre bien avec votre moteur

Comme vous construisez un moteur GraphRAG complet en C#/.NET, je vous conseillerais de concevoir le Binder comme un véritable compilateur, inspiré de **Roslyn** plutôt que de Neo4j :

* **AST** : uniquement la syntaxe produite par ANTLR.
* **Binder** : résolution des noms, des types et des portées, production d'un **Bound AST** immuable.
* **Logical Planner** : conversion du Bound AST en opérateurs logiques (`NodeScan`, `Expand`, `Filter`, `Projection`, etc.).
* **Optimizer** : réécriture du plan logique.
* **Physical Planner** : choix des opérateurs physiques.
* **Execution Engine** : exécution du plan.

Cette séparation présente un avantage majeur : le planificateur logique et l'optimiseur n'ont jamais à manipuler des chaînes de caractères ou à consulter le schéma. Toutes les informations sémantiques (types, symboles, propriétés, diagnostics) sont déjà résolues par le Binder, ce qui simplifie considérablement les étapes suivantes.

À mon avis, c'est la meilleure base pour un moteur GraphRAG évolutif et performant.


# can you write the SymbolTable class in C#

Absolutely. Since you're building a **database query compiler** rather than a general-purpose language compiler, I would not implement `SymbolTable` as just a `Dictionary<string, Symbol>`. It should support:

* nested scopes
* fast lookup
* symbol shadowing (`WITH` in Cypher)
* multiple symbol kinds
* unique IDs
* efficient enumeration
* future optimizer support

This is the design I would use.

## Interfaces

```csharp
namespace GraphRAG.Query.Binding;

public interface ISymbol
{
    int Id { get; }

    string Name { get; }
}
```

---

## Base Symbol

```csharp
namespace GraphRAG.Query.Binding;

public abstract class Symbol : ISymbol
{
    protected Symbol(
        int id,
        string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; }

    public string Name { get; }

    public override string ToString()
        => $"{GetType().Name}({Name})";
}
```

---

## SymbolTable

```csharp
using System.Collections;

namespace GraphRAG.Query.Binding;

public sealed class SymbolTable : IEnumerable<Symbol>
{
    private readonly Dictionary<string, Symbol> _symbols =
        new(StringComparer.OrdinalIgnoreCase);

    private int _nextId = 1;

    public int Count => _symbols.Count;

    public int NextId()
        => _nextId++;

    public bool Contains(string name)
        => _symbols.ContainsKey(name);

    public bool TryLookup(
        string name,
        out Symbol symbol)
    {
        return _symbols.TryGetValue(name, out symbol!);
    }

    public T? Lookup<T>(string name)
        where T : Symbol
    {
        if (!_symbols.TryGetValue(name, out var symbol))
            return null;

        return symbol as T;
    }

    public Symbol Lookup(string name)
    {
        if (!_symbols.TryGetValue(name, out var symbol))
            throw new KeyNotFoundException(name);

        return symbol;
    }

    public void Declare(Symbol symbol)
    {
        if (_symbols.ContainsKey(symbol.Name))
            throw new InvalidOperationException(
                $"Symbol '{symbol.Name}' already declared.");

        _symbols.Add(symbol.Name, symbol);
    }

    public bool Remove(string name)
    {
        return _symbols.Remove(name);
    }

    public void Clear()
    {
        _symbols.Clear();
        _nextId = 1;
    }

    public IEnumerator<Symbol> GetEnumerator()
        => _symbols.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}
```

---

# VariableSymbol example

```csharp
namespace GraphRAG.Query.Binding;

public sealed class VariableSymbol : Symbol
{
    public VariableSymbol(
        int id,
        string name,
        GraphType type)
        : base(id, name)
    {
        Type = type;
    }

    public GraphType Type { get; }
}
```

---

# Usage

```csharp
var table = new SymbolTable();

var person = new VariableSymbol(
    table.NextId(),
    "p",
    GraphTypes.Node);

table.Declare(person);

if (table.TryLookup("p", out var symbol))
{
    Console.WriteLine(symbol.Id);
}
```

---

# But there is one problem...

This implementation is **not enough** for Cypher.

Consider

```cypher
MATCH (p:Person)

CALL
{
    MATCH (p:Movie)
    RETURN p
}

RETURN p
```

The inner `p` hides the outer `p`.

A single dictionary cannot represent this.

Instead, I would use this hierarchy:

```
BinderContext
        │
        ▼
Scope
        │
        ├── Dictionary<string, Symbol>
        │
        └── Parent Scope
```

The `SymbolTable` then becomes responsible only for **ID generation and global symbol storage**, while each `Scope` handles name resolution.

```
SymbolTable
│
├── Symbol #1
├── Symbol #2
├── Symbol #3
├── Symbol #4
└── ...
```

```
Scope #1
    p -> Symbol #1

        │

Scope #2
    p -> Symbol #4
```

Lookup walks upward through the scope chain:

```
Current Scope

↓

Dictionary

↓

Parent Scope

↓

Parent Scope

↓

...
```

This is exactly how Roslyn, C# compilers, SQL Server, and Neo4j organize symbol resolution.

## The architecture I recommend

For a production-quality GraphRAG engine, I would actually split responsibilities further:

* **`SymbolTable`**: owns every symbol created during binding, assigns unique IDs, and allows lookup by ID (useful for later optimizer and execution phases).
* **`Scope`**: maps names to symbols and implements lexical scoping and shadowing.
* **`ScopeManager`**: manages entering and leaving scopes (`MATCH`, `WITH`, `CALL`, subqueries, etc.).
* **`BinderContext`**: ties everything together and exposes the current scope.

This separation scales much better than trying to make a single `SymbolTable` responsible for both symbol ownership and scope resolution. It also makes later stages like the logical planner and optimizer much simpler because every variable reference already points to a unique symbol ID instead of requiring repeated name lookups.
