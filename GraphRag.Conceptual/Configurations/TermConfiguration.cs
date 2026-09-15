using GraphRag.Conceptual.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphRag.Conceptual.Configurations;

public class TermConfiguration : ConceptualEntityConfiguration<Term>
{
    public override void Configure(EntityTypeBuilder<Term> builder)
    {
        base.Configure(builder);

        builder.ToTable("Term");
        builder.Property(t => t.SurfaceForm).HasMaxLength(256).IsRequired();
        builder.Property(t => t.Kind).HasConversion<string>().HasMaxLength(24);
        builder.Property(t => t.Language).HasMaxLength(8);
        builder.Property(t => t.Context).HasMaxLength(128);

        builder.HasIndex(t => t.SurfaceForm);
        builder.HasIndex(t => new { t.SurfaceForm, t.Language, t.Context });

        builder.HasOne(t => t.Concept)
            .WithMany(c => c.Terms)
            .HasForeignKey(t => t.ConceptId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
