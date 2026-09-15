using GraphRag.ITSM.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphRag.ITSM.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> b)
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

        b.ConfigureEntityBase();
    }
}

public class IncidentConfiguration : IEntityTypeConfiguration<Incident>
{
    public void Configure(EntityTypeBuilder<Incident> b)
    {
        b.HasOne(i => i.CausedByProblem).WithMany()
            .HasForeignKey(i => i.CausedByProblemId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(i => i.ResolvedByChange).WithMany()
            .HasForeignKey(i => i.ResolvedByChangeId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class ServiceRequestConfiguration : IEntityTypeConfiguration<ServiceRequest>
{
    public void Configure(EntityTypeBuilder<ServiceRequest> b)
    {
        b.HasOne(s => s.RequestedCatalogItem).WithMany()
            .HasForeignKey(s => s.RequestedCatalogItemId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class ChangeRequestConfiguration : IEntityTypeConfiguration<ChangeRequest>
{
    public void Configure(EntityTypeBuilder<ChangeRequest> b)
    {
        b.Property(c => c.RiskLevel).HasConversion<string>().HasMaxLength(16);
        b.Property(c => c.ChangeType).HasConversion<string>().HasMaxLength(16);
    }
}

public class TicketTaskConfiguration : IEntityTypeConfiguration<TicketTask>
{
    public void Configure(EntityTypeBuilder<TicketTask> b)
    {
        b.HasOne(t => t.ParentTicket).WithMany()
            .HasForeignKey(t => t.ParentTicketId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class TicketAffectedCIConfiguration : IEntityTypeConfiguration<TicketAffectedCI>
{
    public void Configure(EntityTypeBuilder<TicketAffectedCI> b)
    {
        b.HasKey(x => new { x.TicketId, x.ConfigurationItemId });

        b.HasOne(x => x.Ticket).WithMany(t => t.AffectedConfigurationItems)
            .HasForeignKey(x => x.TicketId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.ConfigurationItem).WithMany(c => c.Tickets)
            .HasForeignKey(x => x.ConfigurationItemId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class TicketAffectedServiceConfiguration : IEntityTypeConfiguration<TicketAffectedService>
{
    public void Configure(EntityTypeBuilder<TicketAffectedService> b)
    {
        b.HasKey(x => new { x.TicketId, x.ServiceId });

        b.HasOne(x => x.Ticket).WithMany(t => t.AffectedServices)
            .HasForeignKey(x => x.TicketId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Service).WithMany(s => s.Tickets)
            .HasForeignKey(x => x.ServiceId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class TicketRelationConfiguration : IEntityTypeConfiguration<TicketRelation>
{
    public void Configure(EntityTypeBuilder<TicketRelation> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.RelationKind).HasConversion<string>().HasMaxLength(32);
        b.HasIndex(x => new { x.SourceTicketId, x.TargetTicketId, x.RelationKind }).IsUnique();

        b.HasOne(x => x.SourceTicket).WithMany(t => t.RelatedFrom)
            .HasForeignKey(x => x.SourceTicketId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.TargetTicket).WithMany(t => t.RelatedTo)
            .HasForeignKey(x => x.TargetTicketId).OnDelete(DeleteBehavior.Restrict);
    }
}
