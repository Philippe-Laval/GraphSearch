> Can you read the file "Text_To_SQL_Ontology.md" and work on "ITSM Text-to-SQL". Generated the C# classes for the concept described on the file

I'll design the C# classes for the ITSM Text-to-SQL ontology, structured around the six layers described in the file with the ITSM-specific business concepts. Let me create a well-organized set of files.

Here's a summary of what was created:

# ITSM Text-to-SQL Ontology — C# class model
Created under GraphRag.Ontology\Itsm\, organized by the six layers described in the document:
- Core — OntologyEntity, BusinessConcept, Provenance (identity, labels, provenance, versioning).
- Business (conceptual layer, ITSM-specialized):
	-	Business\WorkItem.cs — WorkItem, Ticket, Incident, ServiceRequest, Problem, ChangeRequest, TicketTask, KnowledgeCase
	-	Business\Party\ — Party, User, Requester, Agent, AssignmentGroup, SupportTeam, Vendor, Organization
	-	Business\Service\ — Service, BusinessService, TechnicalService, ServiceOffering, ServiceCatalogItem
	-	Business\Configuration\ — ConfigurationItem, Application, Server, DatabaseCI, NetworkDevice, CloudResource
	-	Business\Classification\ — Category, Status, Priority, Impact, Urgency, Severity kept distinct (per the doc's warning)
	-	Business\Events\ — OperationalEvent, StatusTransition, AssignmentEvent, Escalation, Reassignment, Resolution, Reopen, Closure
	-	Business\TimeSemantics\ — TicketTimestamps, TimePeriod, BusinessCalendar
	-	Business\Sla\ — ServiceLevelAgreement (Response/Resolution/Availability), SlaMeasurement, SlaBreachStatus
	-	Business\Metrics\ — BusinessMetric with grain/qualifying population/time basis; TicketVolume, Backlog, MTTR, ReopenRate, SLAComplianceRate, etc.
- Data (physical layer) — Database, Schema, Table, View, Column, ForeignKey, Index.
- Mapping (semantic mapping layer) — ConceptRepresentation (concept → asset + optional filter, e.g. ticket WHERE ticket_type='incident'), JoinRelationship with cardinality & preferred paths.
- Linguistic layer — Term (surface form, MapsTo, confidence, context) instead of naive "synonym = concept".
- Query-semantics layer — SemanticQuery, QueryIntent, Filter, Aggregation, Grouping, Ranking, TimeConstraint (with DateBasis to pick CreatedAt vs ResolvedAt), Join.
- SQL layer — SqlDialect, SqlExpression, SqlFunction, SqlQuery, MetricImplementation (bridges metric → dialect SQL).
- Governance layer — DataClassification (PII/PHI/PCI), AccessPolicy, DataOwner, QualityConstraint.

Key design decisions faithful to the document:
-	Business concepts never are their physical tables — they're connected via ConceptRepresentation.
-	Metrics are first-class with explicit grain / qualifying population / time basis / calendar.
-	Priority ≠ Impact ≠ Urgency ≠ Severity.
-	Multiple ticket timestamps are modeled distinctly to answer "sales in 2025"-style temporal questions.
-	Every entity carries Provenance + Version for traceable, versioned assertions.
