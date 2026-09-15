using GraphRag.Conceptual.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphRag.Conceptual.Configurations;

/// <summary>
/// Base configuration that applies the common <see cref="ConceptualEntity"/>
/// property conventions (Uri, PreferredLabel, OntologyVersion, Source, AssertedBy, RowVersion).
/// </summary>
public abstract class ConceptualEntityConfiguration<T> : IEntityTypeConfiguration<T>
    where T : ConceptualEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.Property(e => e.Uri).HasMaxLength(256).IsRequired();
        builder.Property(e => e.PreferredLabel).HasMaxLength(256);
        builder.Property(e => e.OntologyVersion).HasMaxLength(32);
        builder.Property(e => e.Source).HasMaxLength(128);
        builder.Property(e => e.AssertedBy).HasMaxLength(128);
        builder.Property(e => e.RowVersion).IsRowVersion();
    }
}
