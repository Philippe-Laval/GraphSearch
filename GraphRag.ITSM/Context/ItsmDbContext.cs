using GraphRag.ITSM.Entities;
using Microsoft.EntityFrameworkCore;

namespace GraphRag.ITSM.Context;

/// <summary>
/// EF Core 10 DbContext for the ITSM domain.
///
/// Design notes:
/// * Ticket, ConfigurationItem, Service, OperationalEvent use TPH — matches the
///   ontology guidance ("a real DB may have one ticket table with ticket_type").
/// * Classifications (Status, Priority, Impact, Urgency, Severity, Category) are
///   lookup tables so the same value can be referenced across millions of tickets.
/// * Many-to-many links (Ticket↔CI, Ticket↔Service, Agent↔Group, CI↔CI, ...) use
///   explicit join entities so that useful attributes (IsPrimary, timestamps, kind)
///   can be carried on the edge.
/// * Timestamps on Ticket are indexed to support the frequent time-based analytics.
/// </summary>
public class ItsmDbContext : DbContext
{
    public ItsmDbContext(DbContextOptions<ItsmDbContext> options) : base(options) { }

    // ----- Party -----
    public DbSet<Person> People => Set<Person>();
    public DbSet<Agent> Agents => Set<Agent>();
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<AssignmentGroup> AssignmentGroups => Set<AssignmentGroup>();
    public DbSet<AgentGroupMembership> AgentGroupMemberships => Set<AgentGroupMembership>();
    public DbSet<Vendor> Vendors => Set<Vendor>();

    // ----- Service -----
    public DbSet<Service> Services => Set<Service>();
    public DbSet<BusinessService> BusinessServices => Set<BusinessService>();
    public DbSet<TechnicalService> TechnicalServices => Set<TechnicalService>();
    public DbSet<ServiceOffering> ServiceOfferings => Set<ServiceOffering>();
    public DbSet<ServiceCatalogItem> ServiceCatalogItems => Set<ServiceCatalogItem>();
    public DbSet<ServiceSupportGroup> ServiceSupportGroups => Set<ServiceSupportGroup>();
    public DbSet<TechnicalServiceEnablement> TechnicalServiceEnablements => Set<TechnicalServiceEnablement>();
    public DbSet<BusinessServiceConsumer> BusinessServiceConsumers => Set<BusinessServiceConsumer>();

    // ----- Configuration Items -----
    public DbSet<ConfigurationItem> ConfigurationItems => Set<ConfigurationItem>();
    public DbSet<Application> Applications => Set<Application>();
    public DbSet<Server> Servers => Set<Server>();
    public DbSet<DatabaseCI> DatabaseCIs => Set<DatabaseCI>();
    public DbSet<NetworkDevice> NetworkDevices => Set<NetworkDevice>();
    public DbSet<CloudResource> CloudResources => Set<CloudResource>();
    public DbSet<ConfigurationItemSupport> ConfigurationItemSupports => Set<ConfigurationItemSupport>();
    public DbSet<ConfigurationItemDependency> ConfigurationItemDependencies => Set<ConfigurationItemDependency>();

    // ----- Classification -----
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Status> Statuses => Set<Status>();
    public DbSet<Impact> Impacts => Set<Impact>();
    public DbSet<Urgency> Urgencies => Set<Urgency>();
    public DbSet<Priority> Priorities => Set<Priority>();
    public DbSet<Severity> Severities => Set<Severity>();

    // ----- Tickets -----
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<Incident> Incidents => Set<Incident>();
    public DbSet<ServiceRequest> ServiceRequests => Set<ServiceRequest>();
    public DbSet<Problem> Problems => Set<Problem>();
    public DbSet<ChangeRequest> ChangeRequests => Set<ChangeRequest>();
    public DbSet<TicketTask> TicketTasks => Set<TicketTask>();
    public DbSet<TicketAffectedCI> TicketAffectedCIs => Set<TicketAffectedCI>();
    public DbSet<TicketAffectedService> TicketAffectedServices => Set<TicketAffectedService>();
    public DbSet<TicketRelation> TicketRelations => Set<TicketRelation>();

    // ----- Events -----
    public DbSet<OperationalEvent> OperationalEvents => Set<OperationalEvent>();
    public DbSet<StatusTransition> StatusTransitions => Set<StatusTransition>();
    public DbSet<AssignmentEvent> AssignmentEvents => Set<AssignmentEvent>();
    public DbSet<EscalationEvent> EscalationEvents => Set<EscalationEvent>();
    public DbSet<ReassignmentEvent> ReassignmentEvents => Set<ReassignmentEvent>();
    public DbSet<ResolutionEvent> ResolutionEvents => Set<ResolutionEvent>();
    public DbSet<ReopenEvent> ReopenEvents => Set<ReopenEvent>();
    public DbSet<ClosureEvent> ClosureEvents => Set<ClosureEvent>();

    // ----- SLA -----
    public DbSet<ServiceLevelAgreement> ServiceLevelAgreements => Set<ServiceLevelAgreement>();
    public DbSet<SlaMeasurement> SlaMeasurements => Set<SlaMeasurement>();
    public DbSet<BusinessCalendar> BusinessCalendars => Set<BusinessCalendar>();
    public DbSet<CalendarHoliday> CalendarHolidays => Set<CalendarHoliday>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ItsmDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }


    /// <summary>Automatically maintains CreatedAt / UpdatedAt audit stamps.</summary>
    public override int SaveChanges()
    {
        StampAudit();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        StampAudit();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void StampAudit()
    {
        var now = DateTimeOffset.UtcNow;
        foreach (var entry in ChangeTracker.Entries<EntityBase>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    if (entry.Entity.CreatedAt == default)
                        entry.Entity.CreatedAt = now;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    break;
            }
        }
    }
}
