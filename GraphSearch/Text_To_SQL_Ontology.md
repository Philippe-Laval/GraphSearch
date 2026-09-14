> I would like to work on a ontology for the domain "Text to SQL". What is the best practices

For a **Text-to-SQL ontology**, the most important design principle is to avoid treating it as merely an ontology of database schemas. A useful ontology should sit between **how users talk about the business** and **how the database represents it**.

That distinction matters even more in modern enterprise Text-to-SQL. Benchmarks such as Spider 2.0 now include databases with thousands of columns, multiple SQL dialects, project documentation, transformations, and multi-query workflows, so simple `Table → Column → ForeignKey` metadata is no longer enough. ([Spider 2.0][1])

I would structure the ontology into roughly **six layers**.

### 1. Business / conceptual layer

This is the layer users naturally refer to.

For example:

```text
Customer
Order
Product
Employee
Region
Revenue
Profit
Sales
```

These should be concepts independent of their physical database representation.

Relationships might include:

```text
Customer --places--> Order
Order --contains--> Product
Employee --worksIn--> Region
Order --generates--> Revenue
```

This separation is critical. A user asks:

> "What were our top customers last quarter?"

They usually do **not** know whether "customer" corresponds to:

```text
crm_accounts
customer_master
party_dim
```

or a combination of several tables.

Your ontology should bridge that gap.

---

### 2. Physical database layer

Represent the actual schema explicitly.

For example:

```text
Database
 └── Schema
      └── Table
           ├── Column
           ├── PrimaryKey
           ├── ForeignKey
           └── Index
```

Useful relations include:

```text
Database contains Schema
Schema contains Table
Table hasColumn Column

Column hasDataType DataType
Column references Column
Column isPrimaryKey true/false
Column isNullable true/false
```

Then connect this physical model to the conceptual model:

```text
customer_master.customer_id
    represents → CustomerIdentifier

customer_master.customer_name
    represents → CustomerName

orders.net_amount
    represents → NetRevenue
```

This **semantic mapping layer** is arguably the most valuable part of the ontology.

OWL itself is well suited to representing classes, properties, individuals and relationships, and W3C recommends distinguishing classes, individuals, object properties and datatype properties explicitly rather than treating everything as generic labels. ([W3C][2])

---

### 3. Semantic / business-metric layer

Text-to-SQL systems frequently fail not because they cannot write SQL, but because they misunderstand what something means.

Consider:

> "Show revenue by country."

What is `revenue`?

It could be:

```text
SUM(invoice.total)
SUM(order.amount)
SUM(order.amount - discount)
SUM(payment.amount)
```

Therefore model **metrics explicitly**.

For example:

```text
BusinessMetric
    Revenue
    NetRevenue
    GrossRevenue
    Profit
    Margin
    AverageOrderValue
```

And define them:

```text
NetRevenue
    aggregationFunction = SUM
    expression = order.net_amount
    grain = Order
    currency = EUR
```

You can go further:

```text
GrossMargin
    numerator = GrossProfit
    denominator = Revenue
    calculation = numerator / denominator
```

This makes the ontology partly a **semantic layer**, which is highly valuable for Text-to-SQL.

---

### 4. Linguistic layer

This is something I strongly recommend adding specifically for Text-to-SQL.

A concept should have multiple natural-language realizations.

For example:

```text
Customer
    preferredLabel = "customer"
    synonyms =
        client
        account
        buyer
        purchaser
```

Likewise:

```text
Revenue
    synonyms =
        sales
        turnover
        income
```

But be careful with synonyms: they are often **context-dependent rather than truly equivalent**.

For example:

```text
sales
```

might mean:

```text
Revenue
SalesOrder
SalesDepartment
UnitsSold
```

So preferably represent something like:

```text
Term
 ├── surfaceForm
 ├── mapsTo
 ├── confidence
 ├── context
 └── source
```

Rather than simply:

```text
sales = revenue
```

SKOS-style `preferredLabel`, `alternativeLabel`, and broader/narrower relationships can be useful here, while OWL handles the more formal semantics.

---

### 5. Query-semantics layer

This is where an ontology designed specifically for Text-to-SQL becomes much more powerful than a standard data catalog.

Represent concepts such as:

```text
QueryIntent
Filter
Aggregation
Grouping
Ordering
Limit
Comparison
TimeRange
Ranking
Join
SetOperation
```

For example:

> "Top 10 customers by revenue in France last year"

could become:

```text
Intent: Ranking

TargetEntity:
    Customer

Metric:
    Revenue

Ranking:
    descending
    limit = 10

Filter:
    Customer.country = France

TimeRange:
    PreviousYear
```

Only after this representation should you generate SQL.

Conceptually:

```text
Natural language
      ↓
Semantic query
      ↓
Ontology grounding
      ↓
Database grounding
      ↓
SQL
```

This decomposition is much easier to debug than:

```text
Natural language → LLM → SQL
```

---

### 6. Governance and constraint layer

This is often missing but becomes essential in production.

Model things like:

```text
PIIData
SensitiveData
DeprecatedColumn
RestrictedTable
AllowedAggregation
DataOwner
DataDomain
Freshness
QualityLevel
```

For example:

```text
customer.email
    classification = PII
    owner = CRMTeam

employee.salary
    classification = Confidential
    allowedFor = HRAnalyst
```

Then the Text-to-SQL system can reason not only about:

> "Can I generate this query?"

but also:

> "Should this user be allowed to generate this query?"

---

## A good top-level ontology

I would probably start with something like this:

```text
Thing

├── BusinessConcept
│   ├── Entity
│   ├── Attribute
│   ├── Metric
│   ├── Dimension
│   └── Event
│
├── DataAsset
│   ├── Database
│   ├── Schema
│   ├── Table
│   ├── Column
│   └── View
│
├── QueryConcept
│   ├── Query
│   ├── Filter
│   ├── Aggregation
│   ├── Grouping
│   ├── Ranking
│   ├── Join
│   └── TimeConstraint
│
├── LinguisticConcept
│   ├── Term
│   ├── Synonym
│   └── Abbreviation
│
├── SQLConcept
│   ├── SQLExpression
│   ├── SQLFunction
│   ├── SQLDialect
│   └── SQLQuery
│
└── GovernanceConcept
    ├── DataClassification
    ├── AccessPolicy
    ├── DataOwner
    └── QualityConstraint
```

Then define relationships such as:

```text
Entity hasAttribute Attribute
Metric calculatedFrom Attribute

BusinessConcept representedBy DataAsset
Table hasColumn Column

Term denotes BusinessConcept

Query references BusinessConcept
Query hasFilter Filter
Query hasMetric Metric

Column joinsWith Column
Table joinsWith Table

SQLExpression implements Metric
SQLQuery implements Query
```

That last distinction is particularly useful:

```text
Revenue      ← business concept
SUM(o.total) ← SQL implementation
```

Never make those two the same object.

---

## Some particularly important modeling rules

I would follow these practices:

1. **Model semantics before schema.** Start with `Customer`, `Order`, `Revenue`, etc., then map them to tables and columns.

2. **Keep conceptual and physical models separate.** `Customer` should not be synonymous with `dim_customer`.

3. **Treat metrics as first-class objects.** Definitions of revenue, churn, active customer, etc. are often more important than column definitions.

4. **Model join semantics, not just foreign keys.** For example:

```text
Order → Customer
relationship = many-to-one
joinCondition = orders.customer_id = customers.id
preferredJoin = true
```

Also capture cases where valid joins do not have declared foreign keys.

5. **Represent grain/cardinality.** This prevents one of the most dangerous Text-to-SQL problems: duplicate aggregation.

```text
orders
grain = one row per Order

order_items
grain = one row per Product per Order
```

6. **Represent temporal semantics explicitly.**

```text
OrderDate
ShipmentDate
InvoiceDate
FiscalYear
CalendarYear
```

A user saying "sales in 2025" may require knowing which date dimension is appropriate.

7. **Keep provenance.** For ontology assertions, capture where the information came from:

```text
source = DBA
source = documentation
source = inferred
confidence = 0.83
```

8. **Version the ontology.** Database structures and business terminology change. OWL explicitly supports ontology identification/versioning concepts, and version management should be part of your design rather than an afterthought. ([W3C][2])

---

## One thing I would *not* do

I would not try to put absolutely everything into OWL.

For a production Text-to-SQL system, I would use a **hybrid ontology/knowledge graph**:

```text
              Text-to-SQL Knowledge Model

                    Business Ontology
                           │
          ┌────────────────┼─────────────────┐
          ↓                ↓                 ↓
     Vocabulary       Metrics         Relationships
          │                │                 │
          └────────────────┼─────────────────┘
                           ↓
                    Semantic Layer
                           │
                           ↓
                    Schema Mapping
                           │
                           ↓
              Physical Database Metadata
```

Then use:

* **OWL/RDF** for concepts and semantic relationships.
* **SKOS** for terminology, labels and synonyms.
* **SHACL** for validation/constraints.
* A graph database or RDF store for traversal.
* Embeddings for fuzzy terminology/schema matching.
* Database metadata for actual physical schema.
* An LLM for interpreting natural language and constructing the intermediate semantic query.

The formal ontology gives you deterministic knowledge; embeddings and LLMs handle ambiguity.

This aligns well with the direction of current Text-to-SQL evaluation: BIRD explicitly tests database-grounded Text-to-SQL involving external/domain knowledge, while Spider 2.0 shows that real enterprise tasks often require schema metadata, documentation, dialect knowledge, and project-level context rather than SQL syntax alone. ([bird-bench.github.io][3])

### Where I would start

Before defining OWL classes, I would create **20–50 competency questions**—questions your ontology must be able to answer. For example:

```text
What database field represents Customer?
Which tables can be used to calculate Revenue?
What is the preferred join path from Customer to Order?
What does "sales" mean in the Finance domain?
What is the grain of order_items?
Which date should be used for monthly revenue?
What SQL expression implements Net Revenue?
Which dimensions can Revenue be grouped by?
Which columns contain PII?
Which SQL dialect does this database support?
```

Those competency questions will tell you what concepts and relations your ontology actually needs. It is much safer than designing a large class hierarchy first.

If you want to work through this systematically, the next useful step would be to define a **Text-to-SQL Ontology v0.1**, with perhaps **20–30 core classes and 30–40 relationships**, and then test it against 10 concrete natural-language questions and their SQL. I can help you design that ontology next, including a visual class model and an initial OWL/RDF representation.

[1]: https://spider2-sql.github.io/?utm_source=chatgpt.com "Spider 2.0"
[2]: https://www.w3.org/TR/owl-guide/?utm_source=chatgpt.com "OWL Web Ontology Language Guide"
[3]: https://bird-bench.github.io/?utm_source=chatgpt.com "BIRD-bench"


> Can you adapt the business / conceptual layer to handle ITSM and tickets management

Yes. For ITSM and ticket management, I’d adapt the conceptual layer around **work items, services, configuration items, people, organizations, events, and operational metrics** rather than generic commerce entities.

A good starting structure would be:

```text
BusinessConcept

├── WorkItem
│   ├── Ticket
│   │   ├── Incident
│   │   ├── ServiceRequest
│   │   ├── Problem
│   │   ├── ChangeRequest
│   │   └── Task
│   └── KnowledgeCase
│
├── Party
│   ├── User
│   ├── Requester
│   ├── Agent
│   ├── AssignmentGroup
│   ├── SupportTeam
│   ├── Vendor
│   └── Organization
│
├── ServiceConcept
│   ├── BusinessService
│   ├── TechnicalService
│   ├── ServiceOffering
│   └── ServiceCatalogItem
│
├── ConfigurationConcept
│   ├── ConfigurationItem
│   ├── Application
│   ├── Server
│   ├── Database
│   ├── NetworkDevice
│   └── CloudResource
│
├── ClassificationConcept
│   ├── Category
│   ├── Subcategory
│   ├── Priority
│   ├── Impact
│   ├── Urgency
│   ├── Severity
│   └── Status
│
├── OperationalEvent
│   ├── AssignmentEvent
│   ├── StatusChange
│   ├── Escalation
│   ├── Reassignment
│   ├── Resolution
│   ├── Reopen
│   └── Closure
│
├── TimeConcept
│   ├── CreatedAt
│   ├── AssignedAt
│   ├── ResolvedAt
│   ├── ClosedAt
│   ├── DueAt
│   └── SLADeadline
│
└── Metric
    ├── TicketVolume
    ├── OpenTicketCount
    ├── Backlog
    ├── ResolutionTime
    ├── ResponseTime
    ├── FirstResponseTime
    ├── MTTR
    ├── ReopenRate
    ├── SLAComplianceRate
    └── EscalationRate
```

The important part is then the relationships.

```text
Ticket --reportedBy--> Requester
Ticket --assignedTo--> Agent
Ticket --assignedToGroup--> AssignmentGroup

Ticket --affects--> BusinessService
Ticket --affects--> ConfigurationItem

Ticket --hasCategory--> Category
Ticket --hasPriority--> Priority
Ticket --hasStatus--> Status

Incident --causedBy--> Problem
Incident --resolvedBy--> ChangeRequest
Problem --knownErrorDocumentedBy--> KnowledgeArticle

ServiceRequest --requests--> ServiceCatalogItem

Ticket --hasEvent--> OperationalEvent
Ticket --governedBy--> SLA

AssignmentGroup --supports--> Service
ConfigurationItem --supports--> Service
```

For Text-to-SQL, I would also make a few ITSM concepts especially explicit.

### Ticket is the superclass, not the business meaning

A physical database may have one large table called:

```text
ticket
```

with a field such as:

```text
ticket_type
```

But your conceptual model should distinguish:

```text
Incident
ServiceRequest
Problem
ChangeRequest
Task
```

because users ask questions such as:

> "How many incidents breached SLA last month?"

and not usually:

> "How many rows where ticket_type = INC...?"

So you want:

```text
Incident
    subclassOf Ticket

representedBy:
    ticket
    WHERE ticket_type = 'incident'
```

This distinction is extremely useful for semantic grounding.

### Model ticket lifecycle explicitly

ITSM queries are very often lifecycle queries.

For example:

> "How long do P1 incidents stay open?"

This isn't just a property of the ticket. It depends on lifecycle events.

I would represent:

```text
TicketLifecycle

Created
Acknowledged
Assigned
InProgress
Pending
Resolved
Closed
Reopened
```

And events such as:

```text
StatusTransition
    fromStatus
    toStatus
    transitionTimestamp
```

This lets you distinguish:

```text
CurrentStatus
```

from:

```text
HistoricalStatus
```

That matters for questions like:

> "How many tickets were in Pending state during August?"

A simple `ticket.status = 'pending'` cannot answer that correctly if the ticket is now closed.

### Treat SLA as its own concept

Don't model SLA merely as:

```text
sla_breached = true
```

A richer model might be:

```text
ServiceLevelAgreement
    ├── ResponseSLA
    ├── ResolutionSLA
    └── AvailabilitySLA
```

with relationships:

```text
Ticket --governedBy--> ServiceLevelAgreement

ServiceLevelAgreement
    hasTargetDuration
    appliesToPriority
    appliesToService
    hasBusinessCalendar
```

Then introduce:

```text
SLAMeasurement
    ticket
    sla
    startTime
    stopTime
    pauseDuration
    targetDuration
    elapsedDuration
    breachStatus
```

This becomes important when someone asks:

> "Which support groups have the worst SLA compliance?"

### Separate priority, severity, impact and urgency

This is one of the most common semantic mistakes in ITSM systems.

Do not collapse:

```text
Priority
Severity
Impact
Urgency
```

They are related but not equivalent.

You might model:

```text
Impact
    High
    Medium
    Low

Urgency
    High
    Medium
    Low

Priority
    P1
    P2
    P3
    P4
```

with something like:

```text
Priority derivedFrom Impact
Priority derivedFrom Urgency
```

depending on the organization's ITSM policy.

Then the Text-to-SQL system can understand that:

> "critical incidents"

might refer to:

```text
Priority = P1
```

or:

```text
Severity = Critical
```

depending on the organization's vocabulary.

### Support organizational semantics

ITSM reporting often depends heavily on ownership and assignment.

I would model separately:

```text
Requester
AffectedUser
OpenedBy
AssignedAgent
AssignmentGroup
Resolver
ResolutionGroup
ServiceOwner
```

For example:

```text
Ticket --requestedBy--> User
Ticket --affectsUser--> User
Ticket --assignedTo--> Agent
Ticket --assignedToGroup--> AssignmentGroup
Ticket --resolvedBy--> Agent
Ticket --resolvedByGroup--> AssignmentGroup
```

This prevents ambiguity in questions like:

> "Tickets from Finance"

Does that mean:

```text
Requester.department = Finance
```

or:

```text
AssignmentGroup.department = Finance
```

or:

```text
AffectedService.owner = Finance
```

Your ontology should preserve these distinctions.

### Model service and CI relationships explicitly

A useful ITSM semantic chain is:

```text
Ticket
   ↓ affects
ConfigurationItem
   ↓ supports
TechnicalService
   ↓ enables
BusinessService
   ↓ usedBy
BusinessUnit
```

Example:

```text
INC00123
    affects → SAP-PROD-DB01

SAP-PROD-DB01
    supports → SAP ERP Production

SAP ERP Production
    enables → Finance ERP Service
```

Then a user can ask:

> "Which business services were most affected by database incidents last quarter?"

even if the ticket itself only contains a CI reference.

### Define business metrics as first-class concepts

For ITSM, I would include definitions such as:

```text
Backlog
    = tickets currently open

CreatedTicketVolume
    = count of tickets created during period

ResolvedTicketVolume
    = count of tickets resolved during period

ResolutionTime
    = resolved_at - created_at

FirstResponseTime
    = first_response_at - created_at

MTTR
    = average resolution time for qualifying incidents

ReopenRate
    = reopened tickets / resolved tickets

SLAComplianceRate
    = tickets meeting SLA / tickets subject to SLA
```

But don't hard-code those formulas globally if different organizations define them differently.

Instead:

```text
Metric
    preferredDefinition
    formula
    grain
    qualifyingPopulation
    timeBasis
    businessCalendar
```

For example:

```text
MTTR
    grain = Incident
    qualifyingPopulation = ResolvedIncident
    timeBasis = BusinessHours
```

That is much safer.

A practical conceptual model could therefore look like this:

```text
                    ITSM Conceptual Model

                         WorkItem
                            │
          ┌─────────────────┼─────────────────┐
          ↓                 ↓                 ↓
      Incident       ServiceRequest        Problem
          │                                   │
          └────────── relatedTo ──────────────┘
          │
       affects
          ↓
 ConfigurationItem
          │
       supports
          ↓
       Service
          │
       ownedBy
          ↓
     SupportTeam


WorkItem
   ├── reportedBy → User
   ├── assignedTo → Agent
   ├── assignedToGroup → SupportTeam
   ├── hasStatus → Status
   ├── hasPriority → Priority
   ├── hasCategory → Category
   ├── governedBy → SLA
   └── hasLifecycleEvent → Event
```

For Text-to-SQL specifically, I would probably define the **core v0.1 ontology** around roughly these concepts:

```text
Ticket
Incident
ServiceRequest
Problem
ChangeRequest
Task

User
Agent
AssignmentGroup
Organization

BusinessService
TechnicalService
ServiceOffering
ConfigurationItem

Category
Priority
Impact
Urgency
Severity
Status

SLA
LifecycleEvent
StatusTransition

Metric
TimePeriod
BusinessCalendar
```

Then test it against competency questions such as:

```text
How many P1 incidents were opened last month?

Which assignment group resolved the most incidents?

Which services had the highest number of SLA breaches?

What is the average resolution time by priority?

How many incidents were reopened after resolution?

Which CIs generate the most recurring incidents?

What percentage of tickets met their resolution SLA?

Which support groups have the largest backlog?

How many incidents affected the Finance business service?

Which problems are associated with more than 10 incidents?
```

If the ontology can ground all of those unambiguously into concepts, relationships, metrics and time semantics, you have a very good foundation for ITSM Text-to-SQL.
