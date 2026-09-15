using GraphRag.Conceptual.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphRag.Conceptual.Configurations;

public class DataAssetRefConfiguration : ConceptualEntityConfiguration<DataAssetRef>
{
    public override void Configure(EntityTypeBuilder<DataAssetRef> builder)
    {
        base.Configure(builder);

        builder.ToTable("DataAssetRef");
        builder.Property(a => a.Kind).HasConversion<string>().HasMaxLength(16);
        builder.Property(a => a.Dialect).HasConversion<string>().HasMaxLength(24);
        builder.Property(a => a.FullyQualifiedName).HasMaxLength(512).IsRequired();
        builder.Property(a => a.DatabaseName).HasMaxLength(128);
        builder.Property(a => a.SchemaName).HasMaxLength(128);
        builder.Property(a => a.TableName).HasMaxLength(128);
        builder.Property(a => a.ColumnName).HasMaxLength(128);
        builder.Property(a => a.DataType).HasMaxLength(64);

        builder.HasIndex(a => a.FullyQualifiedName).IsUnique();
        builder.HasIndex(a => new { a.DatabaseName, a.SchemaName, a.TableName });

        builder.HasOne(a => a.ParentAsset)
            .WithMany(a => a.Children)
            .HasForeignKey(a => a.ParentAssetId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
