using GraphRag.Conceptual.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphRag.Conceptual.Configurations;

public class ConceptConfiguration : ConceptualEntityConfiguration<Concept>
{
    public override void Configure(EntityTypeBuilder<Concept> builder)
    {
        base.Configure(builder);

        builder.ToTable("Concept");
        builder.HasIndex(c => c.Uri).IsUnique();
        builder.HasIndex(c => new { c.Domain, c.Kind });
        builder.Property(c => c.Kind).HasConversion<string>().HasMaxLength(32);
        builder.Property(c => c.Domain).HasMaxLength(64).IsRequired();
        builder.Property(c => c.SynonymsInline).HasMaxLength(2000);

        builder.HasOne(c => c.ParentConcept)
            .WithMany(c => c.Children)
            .HasForeignKey(c => c.ParentConceptId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
