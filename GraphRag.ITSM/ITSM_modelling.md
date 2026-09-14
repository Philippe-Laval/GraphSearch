> given this ITSM Ontology, how to model ITSM classes to be used by Entity Framework core 10 ? Create a GraphRag.ITSM library to regroup the classes and db context

Good design question. Per the ontology document itself, conceptual and physical models must stay separate. So GraphRag.ITSM should hold the persistence entities (EF Core-shaped), not reference the ontology classes. The ontology's ConceptRepresentation is what would map between them later.

Now let me create the entity model. Design decisions:
- TPH (Table-Per-Hierarchy) for Ticket, ConfigurationItem, Service, OperationalEvent — matches the doc's insight that DBs typically have one ticket table with a ticket_type discriminator.
- Lookup tables for Status, Priority, Impact, Urgency, Severity, Category — small, referenced, with seedable values.
- Enums stored as strings for readability.
- Timestamps flattened on Ticket (each one is queryable/indexable).
- Explicit join entities for many-to-many with useful attributes.

Now the DbContext with all the Fluent API configuration (TPH discriminators, unique keys, indexes, skip-navigations for the many self-referencing joins).

 Here's the summary:

# GraphRag.ITSM — EF Core 10 persistence library
Project layout:

```text
GraphRag.ITSM/
├── GraphRag.ITSM.csproj      (net10.0 + Microsoft.EntityFrameworkCore 10.0.0)
├── ItsmDbContext.cs          (DbSets + all Fluent API config)
└── Entities/
    ├── EntityBase.cs         (Guid Id, ExternalId, audit, RowVersion)
    ├── Party.cs              (Person→Agent TPH, Organization, AssignmentGroup, Vendor)
    ├── Service.cs            (Service TPH: Business/Technical/Offering/Catalog)
    ├── ConfigurationItem.cs  (CI TPH: App/Server/DB/Network/Cloud)
    ├── Classification.cs     (Category, Status, Priority, Impact, Urgency, Severity lookups)
    ├── Ticket.cs             (Ticket TPH: Incident/SR/Problem/Change/Task + M:N joins)
    ├── OperationalEvent.cs   (Event TPH: StatusTransition/Assignment/…)
    └── Sla.cs                (SLA, SlaMeasurement, BusinessCalendar, Holidays)
```

## Key EF Core 10 modelling choices — why they matter

| Concern  | Choice  | Why  |
| :------ | :------ | :------ |
| Ticket / CI / Service / Event hierarchies  | TPH with string discriminator (TicketType, CIType, ServiceType, EventType)  | Matches the ontology doc's exact observation ("real DB may have one ticket table with ticket_type") and keeps polymorphic queries fast.  |
| Priority vs Impact vs Urgency vs Severity | Distinct lookup tables | The doc explicitly warns not to collapse them. Priority has FKs to Impact/Urgency (DerivedFrom…). |
| Timestamps | Flattened columns on Ticket (OpenedAt, ResolvedAt, ClosedAt, SlaResolutionDeadline, …) with indexes | Ontology's DateBasis question ("sales in 2025 = created or resolved?") — each timestamp becomes an indexable, groupable column. |
| Status "now" vs "history" | CurrentStatusId on Ticket plus StatusTransition events | Answers "how many tickets were in Pending during August?" which a single status column cannot. |
| M:N with attributes | Explicit join entities (TicketAffectedCI, AgentGroupMembership, ServiceSupportGroup, ConfigurationItemDependency, TicketRelation) | Lets edges carry IsPrimary, AssignedAt, RelationKind, etc. |
| CI ↔ CI dependencies | Self-referencing join ConfigurationItemDependency | Enables the Ticket→CI→TechnicalService→BusinessService chain the doc emphasises. |
| SLA | First-class ServiceLevelAgreement + per-ticket SlaMeasurement | Never store sla_breached = true alone. |
| Enums | Stored as strings (HasConversion<string>()) | Readable in analytical SQL and NL-to-SQL prompts. |
| Concurrency | RowVersion (IsRowVersion()) applied to every EntityBase via a convention loop | Optimistic locking on every table. |
| Auditing | CreatedAt/UpdatedAt set automatically in SaveChanges(Async) | Zero boilerplate at call sites. |
| Delete behavior | Restrict for reference data (Priority, Status, People), Cascade for owned collections (events, join tables) | Prevents accidental data loss on lookup deletes. |
| Analytics indexes | (CurrentStatusId, PriorityId), OpenedAt, ResolvedAt, AssignedGroupId, (TicketId, EventTimestamp) | Directly supports the ontology's competency questions (backlog by group, SLA compliance, etc.). |

## Consumption example

```csharp
services.AddDbContext<ItsmDbContext>(o =>
    o.UseSqlServer(connectionString));   // or UseNpgsql / UseSqlite

// polymorphic query over the TPH hierarchy
var openP1 = await db.Incidents
    .Where(i => i.Priority!.Level == PriorityLevel.P1
             && !i.ResolvedAt.HasValue)
    .CountAsync();
```

## Note on separation from GraphRag.Ontology
Following the doc's rule "Keep conceptual and physical models separate", GraphRag.ITSM intentionally does not reference GraphRag.Ontology. The bridge belongs in a future mapping component using the ontology's ConceptRepresentation type (e.g. Incident concept → Ticket table filtered by TicketType = 'Incident').

## Create the initial migration

```shell
dotnet ef migrations add InitialItsmSchema -p GraphRag.ITSM
```

(You'll need to add a provider like Microsoft.EntityFrameworkCore.SqlServer and Microsoft.EntityFrameworkCore.Design to the startup/app project.)



