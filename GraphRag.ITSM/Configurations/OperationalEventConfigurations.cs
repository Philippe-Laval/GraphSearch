using GraphRag.ITSM.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphRag.ITSM.Configurations;

public class OperationalEventConfiguration : IEntityTypeConfiguration<OperationalEvent>
{
    public void Configure(EntityTypeBuilder<OperationalEvent> b)
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

        b.ConfigureEntityBase();
    }
}

public class StatusTransitionConfiguration : IEntityTypeConfiguration<StatusTransition>
{
    public void Configure(EntityTypeBuilder<StatusTransition> b)
    {
        b.HasOne(s => s.FromStatus).WithMany()
            .HasForeignKey(s => s.FromStatusId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(s => s.ToStatus).WithMany()
            .HasForeignKey(s => s.ToStatusId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class AssignmentEventConfiguration : IEntityTypeConfiguration<AssignmentEvent>
{
    public void Configure(EntityTypeBuilder<AssignmentEvent> b)
    {
        b.HasOne(a => a.FromGroup).WithMany().HasForeignKey(a => a.FromGroupId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(a => a.ToGroup).WithMany().HasForeignKey(a => a.ToGroupId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(a => a.FromAgent).WithMany().HasForeignKey(a => a.FromAgentId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(a => a.ToAgent).WithMany().HasForeignKey(a => a.ToAgentId).OnDelete(DeleteBehavior.Restrict);
    }
}
