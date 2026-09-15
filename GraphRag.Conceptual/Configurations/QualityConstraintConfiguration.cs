using GraphRag.Conceptual.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphRag.Conceptual.Configurations;

public class QualityConstraintConfiguration : ConceptualEntityConfiguration<QualityConstraint>
{
    public override void Configure(EntityTypeBuilder<QualityConstraint> builder)
    {
        base.Configure(builder);

        builder.ToTable("QualityConstraint");
        builder.Property(r => r.QualityLevel).HasMaxLength(32);
        builder.HasIndex(r => r.AssetId).IsUnique();
        builder.HasOne(r => r.Asset).WithMany()
            .HasForeignKey(r => r.AssetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
