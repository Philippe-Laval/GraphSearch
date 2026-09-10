using GraphRag.EFCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphRag.EFCore.Configurations;

public sealed class GraphEntityConfiguration : IEntityTypeConfiguration<GraphEntity>
{
    public void Configure(EntityTypeBuilder<GraphEntity> builder)
    {
        builder.ToTable("GraphEntity");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
            .IsRequired();

        builder.Property(e => e.Type)
            .IsRequired();

        builder.Property(e => e.Description);

        builder.HasIndex(e => e.Name)
            .HasDatabaseName("IX_GraphEntity_Name");

        builder.HasIndex(e => e.Type)
            .HasDatabaseName("IX_GraphEntity_Type");

        builder.HasMany(e => e.Aliases)
            .WithOne(a => a.Entity)
            .HasForeignKey(a => a.EntityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
