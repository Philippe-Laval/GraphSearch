using GraphRag.Conceptual.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphRag.Conceptual.Configurations;

public class DataClassificationRuleConfiguration : ConceptualEntityConfiguration<DataClassificationRule>
{
    public override void Configure(EntityTypeBuilder<DataClassificationRule> builder)
    {
        base.Configure(builder);

        builder.ToTable("DataClassificationRule");
        builder.Property(r => r.Sensitivity).HasConversion<string>().HasMaxLength(16);
        builder.Property(r => r.ComplianceTags).HasMaxLength(256);
        builder.HasIndex(r => new { r.AssetId, r.Sensitivity }).IsUnique();
        builder.HasOne(r => r.Asset).WithMany()
            .HasForeignKey(r => r.AssetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
