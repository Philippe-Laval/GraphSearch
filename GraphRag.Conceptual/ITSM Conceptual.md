> Given the file describing the ITSM ontology and Following the doc's rule "Keep conceptual and physical models separate", GraphRag.ITSM intentionally does not reference GraphRag.Ontology. The bridge belongs in a future mapping component using the ontology's ConceptRepresentation type (e.g. Incident concept → Ticket table filtered by TicketType = 'Incident'). Create a class library to store the conceptual classes et create a db context. Uses EF Core 10.

Good idea — a persistence library for the ontology itself, so ConceptRepresentation, Term, BusinessMetric, JoinRelationship, governance rules, etc. become queryable data that a Text-to-SQL runtime can consult. I'll call it GraphRag.Conceptual — it references GraphRag.Ontology (to reuse the enums as shared vocabulary) but has its own EF-friendly entity shapes.

Now the entities. Design:
-	Flat ConceptualEntity base with audit + provenance columns.
-	Concept is a single table with a Kind column (Entity/Metric/Dimension/Event/Party/Service/CI/Classification/…) — a conceptual metamodel is inherently open, so avoid TPH here.
-	Term, DataAssetRef, ConceptRepresentation, JoinRelationship, BusinessMetricDefinition, MetricImplementation for the mapping bridge.
-	SemanticQuerySample stored as JSON for few-shot Text-to-SQL grounding.
-	CompetencyQuestion — the document explicitly recommends storing these.
-	Governance rules referring to DataAssetRef.


# GraphRag.Conceptual — the ontology-persistence bridge (EF Core 10)

Purpose: Store the conceptual ontology itself as queryable data — so a Text-to-SQL runtime can look up "which table represents Incident?", "what does 'critical' mean here?", "how do I compute MTTR in T-SQL?" at runtime.
Project layout:

```text
GraphRag.Conceptual/
├── GraphRag.Conceptual.csproj   (net10.0, EF Core 10.0.0, ref GraphRag.Ontology)
├── OntologyDbContext.cs
└── Entities/
    ├── ConceptualEntity.cs           (Uri, PreferredLabel, provenance, version, audit, RowVersion)
    ├── Concept.cs                    (open metamodel + ConceptRelation predicate graph)
    ├── Term.cs                       (surface form, kind, language, context, embedding blob)
    ├── DataAssetRef.cs               (registry of tables/columns/views by FQN + dialect)
    ├── ConceptRepresentation.cs      (Concept ⇄ Asset + filter + JoinRelationship)
    ├── BusinessMetricDefinition.cs   (metric definition + per-dialect implementations)
    ├── CompetencyQuestion.cs         (stored questions + SemanticQuerySample JSON for few-shot)
    └── Governance.cs                 (Classification / AccessPolicy / DataOwner / Quality rules)
```


## How this satisfies the doc's design rules

| Doc rule  | How it's honored  |
| :------ | :------ |
| Keep conceptual & physical separate  | This library models the ontology; GraphRag.ITSM is unaware of it. Both can be swapped/versioned independently. |
| Model semantics before schema  | Concept + ConceptRelation are the primary store; DataAssetRef and ConceptRepresentation come after. |
| Metrics as first-class  | BusinessMetricDefinition with Aggregation, Grain, QualifyingPopulation, TimeBasis, BusinessCalendarConcept, plus per-dialect MetricImplementation. |
| Model join semantics  | JoinRelationship: cardinality, IsPreferred, BackedByForeignKey, arbitrary join condition. |
| Represent grain / cardinality  | DataAssetRef.Grain + JoinRelationship.Cardinality. |
| Temporal semantics  | Concept.Kind = TimeConcept + BusinessMetricDefinition.TimeBasis and calendar concept. |
| Provenance  | Every ConceptualEntity has Source, Confidence, AssertedBy, AssertedAt. |
| Version the ontology  | OntologyVersion on every entity + RowVersion for optimistic locking. |
| Synonyms are context-dependent  | Term.Context + Language + composite index (SurfaceForm, Language, Context). |
| Competency-question-first  | CompetencyQuestion + SemanticQuerySample (JSON body) as first-class tables. |
| Governance  | DataClassificationRule, AccessPolicyRule, DataOwnerRecord, QualityConstraint — all keyed by DataAssetRef. |

## Namespace / naming discipline

- References GraphRag.Ontology only for enums (MappingKind, JoinCardinality, TermKind, AggregationFunction, TimeBasis, Sensitivity, SqlDialect, QueryIntent) — the value vocabulary is shared, the entity shapes are EF-native.
- Entity types are named to avoid collisions with the ontology classes (Concept, DataAssetRef, BusinessMetricDefinition, DataClassificationRule…) so both namespaces can be using-imported together without ambiguity. |

## Solution shape after this task

```text
GraphSearch.sln
├── GraphRag.Ontology       ← pure conceptual C# model (in-memory ontology types + enums)
├── GraphRag.Conceptual     ← EF Core persistence for the ontology itself  ★ new
└── GraphRag.ITSM           ← EF Core persistence for operational tickets/CIs/SLAs
```

## Example: registering the two DbContexts side-by-side

```csharp
services.AddDbContext<OntologyDbContext>(o => o.UseSqlServer(ontoConn));
services.AddDbContext<ItsmDbContext>(o => o.UseSqlServer(itsmConn));

// The Text-to-SQL runtime:
// 1) look up NL "critical incidents" in Term → Concept
// 2) find its ConceptRepresentation → DataAssetRef (crm.dbo.ticket + filter)
// 3) find preferred JoinRelationships to compose the FROM/JOIN clauses
// 4) find MetricImplementation for the requested dialect
// 5) check DataClassificationRule + AccessPolicyRule before emitting the SQL
// 6) execute the resulting SQL against ItsmDbContext (or the source system)
```

