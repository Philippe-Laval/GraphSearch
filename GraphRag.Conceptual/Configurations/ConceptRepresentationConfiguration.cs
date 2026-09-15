using GraphRag.Conceptual.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphRag.Conceptual.Configurations;

public class ConceptRepresentationConfiguration : ConceptualEntityConfiguration<ConceptRepresentation>
{
    public override void Configure(EntityTypeBuilder<ConceptRepresentation> builder)
    {
        base.Configure(builder);

        builder.ToTable("ConceptRepresentation");
        builder.Property(r => r.Kind).HasConversion<string>().HasMaxLength(24);
        builder.Property(r => r.FilterExpression).HasMaxLength(2000);
        builder.Property(r => r.Rationale).HasMaxLength(1000);

        builder.HasIndex(r => new { r.ConceptId, r.AssetId, r.Kind }).IsUnique();

        builder.HasOne(r => r.Concept)
            .WithMany(c => c.Representations)
            .HasForeignKey(r => r.ConceptId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(r => r.Asset)
            .WithMany()
            .HasForeignKey(r => r.AssetId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
