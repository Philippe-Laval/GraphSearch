# Services

The DB contains ~50 DbSets across Party, Service, ConfigurationItem, Classification, Ticket, Events, and SLA. I'll create a generic EntityService<T> base for the standard EntityBase-derived types and derived services grouped by domain (one file per area). Join entities with composite keys get dedicated services.

## Generic base (EntityService.cs)
-	EntityService<T> where T : EntityBase — provides GetAllAsync, GetByIdAsync, CreateAsync, UpdateAsync, DeleteAsync with automatic audit-field handling (CreatedAt/UpdatedAt) and preservation of Id/CreatedAt/CreatedBy on updates via Entry.CurrentValues.SetValues.

Domain files — one derived service per concrete DbSet:

| File  | Services  |
| :------ | :------ |
| VendorService.cs  | Vendor (refactored to inherit base)  |
| PartyServices.cs  | Person, Agent, Organization, AssignmentGroup, AgentGroupMembership*  |
| ServiceCatalogServices.cs  | BusinessService, TechnicalService, ServiceOffering, ServiceCatalogItem, ServiceSupportGroup*, TechnicalServiceEnablement*, BusinessServiceConsumer*  |
| ConfigurationItemServices.cs  | Application, Server, DatabaseCI, NetworkDevice, CloudResource, ConfigurationItemSupport*, ConfigurationItemDependency*  |
| ClassificationServices.cs  | Category, Status, Impact, Urgency, Priority, Severity  |
| TicketServices.cs  | Incident, ServiceRequest, Problem, ChangeRequest, TicketTask, TicketAffectedCI*, TicketAffectedService*, TicketRelation  |
| OperationalEventServices.cs  | StatusTransition, AssignmentEvent, EscalationEvent, ReassignmentEvent, ResolutionEvent, ReopenEvent, ClosureEvent  |
| SlaServices.cs  | ServiceLevelAgreement, SlaMeasurement, BusinessCalendar, CalendarHoliday  |

* = composite-key join entities: they get dedicated services with (id1, id2) overloads for GetAsync/DeleteAsync.

## Design notes:
-	Abstract TPH roots (Ticket, Service, ConfigurationItem, OperationalEvent) intentionally have no service — 
	CRUD only makes sense on concrete subtypes. 
	Query them directly via DbContext when polymorphic reads are needed.
-	All methods async with CancellationToken support.
-	Uses AsNoTracking() on reads.