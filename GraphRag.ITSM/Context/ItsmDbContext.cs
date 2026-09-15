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
        // Apply IEntityTypeConfiguration<>s in this assembly.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ItsmDbContext).Assembly);

        ConfigureParty(modelBuilder);
        ConfigureService(modelBuilder);
        ConfigureConfigurationItems(modelBuilder);
        ConfigureClassification(modelBuilder);
        ConfigureTickets(modelBuilder);
        ConfigureEvents(modelBuilder);
        ConfigureSla(modelBuilder);
        ConfigureCommonAudit(modelBuilder);
    }

    private static void ConfigureCommonAudit(ModelBuilder mb)
    {
        // RowVersion + ExternalId conventions on every EntityBase.
        foreach (var entityType in mb.Model.GetEntityTypes())
        {
            if (typeof(EntityBase).IsAssignableFrom(entityType.ClrType))
            {
                mb.Entity(entityType.ClrType)
                    .Property(nameof(EntityBase.RowVersion))
                    .IsRowVersion();

                mb.Entity(entityType.ClrType)
                    .Property(nameof(EntityBase.ExternalId))
                    .HasMaxLength(64);

                mb.Entity(entityType.ClrType)
                    .HasIndex(nameof(EntityBase.ExternalId))
                    .HasDatabaseName($"IX_{entityType.GetTableName()}_ExternalId");
            }
        }
    }

    private static void ConfigureParty(ModelBuilder mb)
    {
        mb.Entity<Person>(b =>
        {
            b.ToTable("Person");
            b.HasDiscriminator<string>("PersonType")
                .HasValue<Person>("User")
                .HasValue<Agent>("Agent");
            b.Property(p => p.FullName).HasMaxLength(200).IsRequired();
            b.Property(p => p.Email).HasMaxLength(320);
            b.HasIndex(p => p.Email);
            b.HasOne(p => p.Organization)
                .WithMany(o => o.Members)
                .HasForeignKey(p => p.OrganizationId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        mb.Entity<Organization>(b =>
        {
            b.Property(o => o.Name).HasMaxLength(200).IsRequired();
            b.HasOne(o => o.ParentOrganization)
                .WithMany(o => o.Children)
                .HasForeignKey(o => o.ParentOrganizationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        mb.Entity<AssignmentGroup>(b =>
        {
            b.Property(g => g.Name).HasMaxLength(200).IsRequired();
            b.HasIndex(g => g.Name).IsUnique();
        });

        mb.Entity<AgentGroupMembership>(b =>
        {
            b.HasKey(m => new { m.AgentId, m.AssignmentGroupId, m.AssignedAt });
            b.HasOne(m => m.Agent)
                .WithMany(a => a.GroupMemberships)
                .HasForeignKey(m => m.AgentId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(m => m.AssignmentGroup)
                .WithMany(g => g.Memberships)
                .HasForeignKey(m => m.AssignmentGroupId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        mb.Entity<Vendor>(b => b.Property(v => v.Name).HasMaxLength(200).IsRequired());
    }

    private static void ConfigureService(ModelBuilder mb)
    {
        mb.Entity<Service>(b =>
        {
            b.ToTable("Service");
            b.HasDiscriminator<string>("ServiceType")
                .HasValue<BusinessService>("Business")
                .HasValue<TechnicalService>("Technical")
                .HasValue<ServiceOffering>("Offering")
                .HasValue<ServiceCatalogItem>("Catalog");
            b.Property(s => s.Name).HasMaxLength(200).IsRequired();
            b.HasIndex(s => s.Name);

            b.HasOne(s => s.Owner)
                .WithMany()
                .HasForeignKey(s => s.OwnerId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        mb.Entity<ServiceOffering>()
            .HasOne(o => o.BusinessService)
            .WithMany()
            .HasForeignKey(o => o.BusinessServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        mb.Entity<ServiceSupportGroup>(b =>
        {
            b.HasKey(x => new { x.ServiceId, x.AssignmentGroupId });
            b.HasOne(x => x.Service).WithMany(s => s.SupportGroups)
                .HasForeignKey(x => x.ServiceId).OnDelete(DeleteBehavior.Cascade);
            b.HasOne(x => x.AssignmentGroup).WithMany(g => g.SupportedServices)
                .HasForeignKey(x => x.AssignmentGroupId).OnDelete(DeleteBehavior.Cascade);
        });

        mb.Entity<TechnicalServiceEnablement>(b =>
        {
            b.HasKey(x => new { x.TechnicalServiceId, x.BusinessServiceId });
            b.HasOne(x => x.TechnicalService).WithMany(s => s.Enables)
                .HasForeignKey(x => x.TechnicalServiceId).OnDelete(DeleteBehavior.NoAction);
            b.HasOne(x => x.BusinessService).WithMany(s => s.EnabledBy)
                .HasForeignKey(x => x.BusinessServiceId).OnDelete(DeleteBehavior.NoAction);
        });

        mb.Entity<BusinessServiceConsumer>(b =>
        {
            b.HasKey(x => new { x.BusinessServiceId, x.OrganizationId });
            b.HasOne(x => x.BusinessService).WithMany(s => s.ConsumedBy)
                .HasForeignKey(x => x.BusinessServiceId).OnDelete(DeleteBehavior.Cascade);
            b.HasOne(x => x.Organization).WithMany()
                .HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureConfigurationItems(ModelBuilder mb)
    {
        mb.Entity<ConfigurationItem>(b =>
        {
            b.ToTable("ConfigurationItem");
            b.HasDiscriminator<string>("CIType")
                .HasValue<Application>("Application")
                .HasValue<Server>("Server")
                .HasValue<DatabaseCI>("Database")
                .HasValue<NetworkDevice>("NetworkDevice")
                .HasValue<CloudResource>("CloudResource");
            b.Property(c => c.Name).HasMaxLength(200).IsRequired();
            b.HasIndex(c => c.Name);
            b.HasIndex(c => c.AssetTag);
            b.Property(c => c.OperationalStatus).HasConversion<string>().HasMaxLength(32);
        });

        mb.Entity<ConfigurationItemSupport>(b =>
        {
            b.HasKey(x => new { x.ConfigurationItemId, x.TechnicalServiceId });
            b.HasOne(x => x.ConfigurationItem).WithMany(c => c.SupportedTechnicalServices)
                .HasForeignKey(x => x.ConfigurationItemId).OnDelete(DeleteBehavior.Cascade);
            b.HasOne(x => x.TechnicalService).WithMany(s => s.SupportedByCIs)
                .HasForeignKey(x => x.TechnicalServiceId).OnDelete(DeleteBehavior.Cascade);
        });

        mb.Entity<ConfigurationItemDependency>(b =>
        {
            b.HasKey(x => new { x.SourceId, x.TargetId });
            b.HasOne(x => x.Source).WithMany(c => c.DependsOn)
                .HasForeignKey(x => x.SourceId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.Target).WithMany(c => c.DependedOnBy)
                .HasForeignKey(x => x.TargetId).OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureClassification(ModelBuilder mb)
    {
        void ConfigureLookup<T>(string tableName) where T : ClassificationLookup
        {
            mb.Entity<T>(b =>
            {
                b.ToTable(tableName);
                b.Property(x => x.Code).HasMaxLength(32).IsRequired();
                b.Property(x => x.Name).HasMaxLength(100).IsRequired();
                b.HasIndex(x => x.Code).IsUnique();
            });
        }

        ConfigureLookup<Category>("Category");
        ConfigureLookup<Status>("Status");
        ConfigureLookup<Impact>("Impact");
        ConfigureLookup<Urgency>("Urgency");
        ConfigureLookup<Priority>("Priority");
        ConfigureLookup<Severity>("Severity");

        mb.Entity<Category>()
            .HasOne(c => c.ParentCategory)
            .WithMany(c => c.Children)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        mb.Entity<Status>().Property(s => s.Kind).HasConversion<string>().HasMaxLength(32);
        mb.Entity<Impact>().Property(s => s.Level).HasConversion<string>().HasMaxLength(16);
        mb.Entity<Urgency>().Property(s => s.Level).HasConversion<string>().HasMaxLength(16);
        mb.Entity<Priority>().Property(s => s.Level).HasConversion<string>().HasMaxLength(16);
        mb.Entity<Severity>().Property(s => s.Level).HasConversion<string>().HasMaxLength(16);

        mb.Entity<Priority>()
            .HasOne(p => p.DerivedFromImpact).WithMany()
            .HasForeignKey(p => p.DerivedFromImpactId).OnDelete(DeleteBehavior.Restrict);
        mb.Entity<Priority>()
            .HasOne(p => p.DerivedFromUrgency).WithMany()
            .HasForeignKey(p => p.DerivedFromUrgencyId).OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureTickets(ModelBuilder mb)
    {
        mb.Entity<Ticket>(b =>
        {
            b.ToTable("Ticket");
            b.HasDiscriminator<string>("TicketType")
                .HasValue<Incident>("Incident")
                .HasValue<ServiceRequest>("ServiceRequest")
                .HasValue<Problem>("Problem")
                .HasValue<ChangeRequest>("ChangeRequest")
                .HasValue<TicketTask>("Task");

            b.Property(t => t.Number).HasMaxLength(32).IsRequired();
            b.HasIndex(t => t.Number).IsUnique();
            b.Property(t => t.ShortDescription).HasMaxLength(500).IsRequired();

            // Frequent analytical filters — index them.
            b.HasIndex(t => t.OpenedAt);
            b.HasIndex(t => t.ResolvedAt);
            b.HasIndex(t => t.ClosedAt);
            b.HasIndex(t => new { t.CurrentStatusId, t.PriorityId });
            b.HasIndex(t => t.AssignedGroupId);
            b.HasIndex(t => t.AssignedAgentId);

            b.HasOne(t => t.Requester).WithMany().HasForeignKey(t => t.RequesterId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(t => t.AffectedUser).WithMany().HasForeignKey(t => t.AffectedUserId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(t => t.AssignedAgent).WithMany().HasForeignKey(t => t.AssignedAgentId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(t => t.AssignedGroup).WithMany().HasForeignKey(t => t.AssignedGroupId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(t => t.Resolver).WithMany().HasForeignKey(t => t.ResolverId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(t => t.ResolutionGroup).WithMany().HasForeignKey(t => t.ResolutionGroupId).OnDelete(DeleteBehavior.Restrict);

            b.HasOne(t => t.CurrentStatus).WithMany().HasForeignKey(t => t.CurrentStatusId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(t => t.Priority).WithMany().HasForeignKey(t => t.PriorityId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(t => t.Impact).WithMany().HasForeignKey(t => t.ImpactId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(t => t.Urgency).WithMany().HasForeignKey(t => t.UrgencyId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(t => t.Severity).WithMany().HasForeignKey(t => t.SeverityId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(t => t.Category).WithMany().HasForeignKey(t => t.CategoryId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(t => t.Subcategory).WithMany().HasForeignKey(t => t.SubcategoryId).OnDelete(DeleteBehavior.Restrict);
        });

        mb.Entity<Incident>(b =>
        {
            b.HasOne(i => i.CausedByProblem).WithMany()
                .HasForeignKey(i => i.CausedByProblemId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(i => i.ResolvedByChange).WithMany()
                .HasForeignKey(i => i.ResolvedByChangeId).OnDelete(DeleteBehavior.Restrict);
        });

        mb.Entity<ServiceRequest>()
            .HasOne(s => s.RequestedCatalogItem).WithMany()
            .HasForeignKey(s => s.RequestedCatalogItemId).OnDelete(DeleteBehavior.Restrict);

        mb.Entity<ChangeRequest>(b =>
        {
            b.Property(c => c.RiskLevel).HasConversion<string>().HasMaxLength(16);
            b.Property(c => c.ChangeType).HasConversion<string>().HasMaxLength(16);
        });

        mb.Entity<TicketTask>()
            .HasOne(t => t.ParentTicket).WithMany()
            .HasForeignKey(t => t.ParentTicketId).OnDelete(DeleteBehavior.Restrict);

        mb.Entity<TicketAffectedCI>(b =>
        {
            b.HasKey(x => new { x.TicketId, x.ConfigurationItemId });
            b.HasOne(x => x.Ticket).WithMany(t => t.AffectedConfigurationItems)
                .HasForeignKey(x => x.TicketId).OnDelete(DeleteBehavior.Cascade);
            b.HasOne(x => x.ConfigurationItem).WithMany(c => c.Tickets)
                .HasForeignKey(x => x.ConfigurationItemId).OnDelete(DeleteBehavior.Cascade);
        });

        mb.Entity<TicketAffectedService>(b =>
        {
            b.HasKey(x => new { x.TicketId, x.ServiceId });
            b.HasOne(x => x.Ticket).WithMany(t => t.AffectedServices)
                .HasForeignKey(x => x.TicketId).OnDelete(DeleteBehavior.Cascade);
            b.HasOne(x => x.Service).WithMany(s => s.Tickets)
                .HasForeignKey(x => x.ServiceId).OnDelete(DeleteBehavior.Cascade);
        });

        mb.Entity<TicketRelation>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.RelationKind).HasConversion<string>().HasMaxLength(32);
            b.HasIndex(x => new { x.SourceTicketId, x.TargetTicketId, x.RelationKind }).IsUnique();
            b.HasOne(x => x.SourceTicket).WithMany(t => t.RelatedFrom)
                .HasForeignKey(x => x.SourceTicketId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.TargetTicket).WithMany(t => t.RelatedTo)
                .HasForeignKey(x => x.TargetTicketId).OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureEvents(ModelBuilder mb)
    {
        mb.Entity<OperationalEvent>(b =>
        {
            b.ToTable("OperationalEvent");
            b.HasDiscriminator<string>("EventType")
                .HasValue<StatusTransition>("StatusTransition")
                .HasValue<AssignmentEvent>("Assignment")
                .HasValue<EscalationEvent>("Escalation")
                .HasValue<ReassignmentEvent>("Reassignment")
                .HasValue<ResolutionEvent>("Resolution")
                .HasValue<ReopenEvent>("Reopen")
                .HasValue<ClosureEvent>("Closure");

            b.HasIndex(e => new { e.TicketId, e.EventTimestamp });

            b.HasOne(e => e.Ticket).WithMany(t => t.Events)
                .HasForeignKey(e => e.TicketId).OnDelete(DeleteBehavior.Cascade);
            b.HasOne(e => e.PerformedBy).WithMany()
                .HasForeignKey(e => e.PerformedById).OnDelete(DeleteBehavior.Restrict);
        });

        mb.Entity<StatusTransition>(b =>
        {
            b.HasOne(s => s.FromStatus).WithMany()
                .HasForeignKey(s => s.FromStatusId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(s => s.ToStatus).WithMany()
                .HasForeignKey(s => s.ToStatusId).OnDelete(DeleteBehavior.Restrict);
        });

        mb.Entity<AssignmentEvent>(b =>
        {
            b.HasOne(a => a.FromGroup).WithMany().HasForeignKey(a => a.FromGroupId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(a => a.ToGroup).WithMany().HasForeignKey(a => a.ToGroupId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(a => a.FromAgent).WithMany().HasForeignKey(a => a.FromAgentId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(a => a.ToAgent).WithMany().HasForeignKey(a => a.ToAgentId).OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureSla(ModelBuilder mb)
    {
        mb.Entity<ServiceLevelAgreement>(b =>
        {
            b.Property(s => s.Name).HasMaxLength(200).IsRequired();
            b.Property(s => s.Kind).HasConversion<string>().HasMaxLength(16);
            b.HasIndex(s => s.Name);

            b.HasOne(s => s.AppliesToPriority).WithMany()
                .HasForeignKey(s => s.AppliesToPriorityId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(s => s.AppliesToService).WithMany()
                .HasForeignKey(s => s.AppliesToServiceId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(s => s.BusinessCalendar).WithMany()
                .HasForeignKey(s => s.BusinessCalendarId).OnDelete(DeleteBehavior.Restrict);
        });

        mb.Entity<SlaMeasurement>(b =>
        {
            b.Property(m => m.BreachStatus).HasConversion<string>().HasMaxLength(16);
            b.HasIndex(m => new { m.TicketId, m.ServiceLevelAgreementId });
            b.HasOne(m => m.Ticket).WithMany(t => t.SlaMeasurements)
                .HasForeignKey(m => m.TicketId).OnDelete(DeleteBehavior.Cascade);
            b.HasOne(m => m.ServiceLevelAgreement).WithMany()
                .HasForeignKey(m => m.ServiceLevelAgreementId).OnDelete(DeleteBehavior.Restrict);
        });

        mb.Entity<BusinessCalendar>(b =>
        {
            b.Property(c => c.Name).HasMaxLength(100).IsRequired();
            b.HasIndex(c => c.Name).IsUnique();
        });

        mb.Entity<CalendarHoliday>(b =>
        {
            b.HasKey(h => h.Id);
            b.HasOne(h => h.BusinessCalendar).WithMany(c => c.Holidays)
                .HasForeignKey(h => h.BusinessCalendarId).OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(h => new { h.BusinessCalendarId, h.Date }).IsUnique();
        });
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
