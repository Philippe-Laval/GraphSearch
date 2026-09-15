using GraphRag.Conceptual.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphRag.Conceptual.Configurations;

public class DataOwnerRecordConfiguration : ConceptualEntityConfiguration<DataOwnerRecord>
{
    public override void Configure(EntityTypeBuilder<DataOwnerRecord> builder)
    {
        base.Configure(builder);

        builder.ToTable("DataOwnerRecord");
        builder.Property(r => r.OwnerTeam).HasMaxLength(128).IsRequired();
        builder.Property(r => r.Steward).HasMaxLength(128);
        builder.Property(r => r.DataDomain).HasMaxLength(64);
        builder.HasIndex(r => r.AssetId).IsUnique();
        builder.HasOne(r => r.Asset).WithMany()
            .HasForeignKey(r => r.AssetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
