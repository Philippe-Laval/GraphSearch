using GraphRag.EFCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphRag.EFCore.Configurations;

public sealed class GraphEntityAliasConfiguration : IEntityTypeConfiguration<GraphEntityAlias>
{
    public void Configure(EntityTypeBuilder<GraphEntityAlias> builder)
    {
        builder.ToTable("GraphEntityAlias");

        builder.HasKey(a => new { a.EntityId, a.Alias });

        builder.Property(a => a.Alias)
            .IsRequired();

        builder.HasIndex(a => a.Alias)
            .HasDatabaseName("IX_GraphEntityAlias_Alias");

        // Defined in GraphEntityConfiguration, so this is commented out to avoid duplicate configuration
        // builder.HasOne(a => a.Entity)
        //    .WithMany(e => e.Aliases)
        //    .HasForeignKey(a => a.EntityId)
        //    .OnDelete(DeleteBehavior.Cascade);
    }
}
