using GraphRag.Conceptual.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphRag.Conceptual.Configurations;

public class ConceptRelationConfiguration : ConceptualEntityConfiguration<ConceptRelation>
{
    public override void Configure(EntityTypeBuilder<ConceptRelation> builder)
    {
        base.Configure(builder);

        builder.ToTable("ConceptRelation");
        builder.Property(r => r.Predicate).HasMaxLength(64).IsRequired();
        builder.Property(r => r.Cardinality).HasConversion<string>().HasMaxLength(16);
        builder.HasIndex(r => new { r.SourceConceptId, r.Predicate, r.TargetConceptId }).IsUnique();

        builder.HasOne(r => r.SourceConcept)
            .WithMany(c => c.OutgoingRelations)
            .HasForeignKey(r => r.SourceConceptId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(r => r.TargetConcept)
            .WithMany(c => c.IncomingRelations)
            .HasForeignKey(r => r.TargetConceptId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
