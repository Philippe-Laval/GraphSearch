using GraphRag.Conceptual.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphRag.Conceptual.Configurations;

public class MetricImplementationConfiguration : ConceptualEntityConfiguration<MetricImplementation>
{
    public override void Configure(EntityTypeBuilder<MetricImplementation> builder)
    {
        base.Configure(builder);

        builder.ToTable("MetricImplementation");
        builder.Property(i => i.Dialect).HasConversion<string>().HasMaxLength(24);
        builder.Property(i => i.Expression).HasMaxLength(4000).IsRequired();
        builder.Property(i => i.RequiredTables).HasMaxLength(1000);

        builder.HasIndex(i => new { i.MetricDefinitionId, i.Dialect }).IsUnique();

        builder.HasOne(i => i.MetricDefinition)
            .WithMany(m => m.Implementations)
            .HasForeignKey(i => i.MetricDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
