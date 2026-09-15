using GraphRag.Conceptual.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphRag.Conceptual.Configurations;

public class BusinessMetricDefinitionConfiguration : ConceptualEntityConfiguration<BusinessMetricDefinition>
{
    public override void Configure(EntityTypeBuilder<BusinessMetricDefinition> builder)
    {
        base.Configure(builder);

        builder.ToTable("BusinessMetricDefinition");
        builder.Property(m => m.Aggregation).HasConversion<string>().HasMaxLength(24);
        builder.Property(m => m.TimeBasis).HasConversion<string>().HasMaxLength(16);
        builder.Property(m => m.PreferredDefinition).HasMaxLength(1000);
        builder.Property(m => m.Formula).HasMaxLength(1000);
        builder.Property(m => m.Grain).HasMaxLength(128);
        builder.Property(m => m.QualifyingPopulation).HasMaxLength(128);
        builder.Property(m => m.Unit).HasMaxLength(32);

        builder.HasIndex(m => m.ConceptId).IsUnique();

        builder.HasOne(m => m.Concept).WithMany()
            .HasForeignKey(m => m.ConceptId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(m => m.BusinessCalendarConcept).WithMany()
            .HasForeignKey(m => m.BusinessCalendarConceptId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
