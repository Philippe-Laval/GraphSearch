using GraphRag.Conceptual.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphRag.Conceptual.Configurations;

public class AccessPolicyRuleConfiguration : ConceptualEntityConfiguration<AccessPolicyRule>
{
    public override void Configure(EntityTypeBuilder<AccessPolicyRule> builder)
    {
        base.Configure(builder);

        builder.ToTable("AccessPolicyRule");
        builder.Property(r => r.AllowedRolesCsv).HasMaxLength(1000);
        builder.Property(r => r.DeniedRolesCsv).HasMaxLength(1000);
        builder.Property(r => r.AllowedAggregationsCsv).HasMaxLength(500);
        builder.HasIndex(r => r.AssetId);
        builder.HasOne(r => r.Asset).WithMany()
            .HasForeignKey(r => r.AssetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
