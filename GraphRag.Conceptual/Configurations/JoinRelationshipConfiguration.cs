using GraphRag.Conceptual.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphRag.Conceptual.Configurations;

public class JoinRelationshipConfiguration : ConceptualEntityConfiguration<JoinRelationship>
{
    public override void Configure(EntityTypeBuilder<JoinRelationship> builder)
    {
        base.Configure(builder);

        builder.ToTable("JoinRelationship");
        builder.Property(j => j.JoinCondition).HasMaxLength(1000).IsRequired();
        builder.Property(j => j.Cardinality).HasConversion<string>().HasMaxLength(16);

        builder.HasIndex(j => new { j.LeftTableId, j.RightTableId });

        builder.HasOne(j => j.LeftTable).WithMany()
            .HasForeignKey(j => j.LeftTableId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(j => j.RightTable).WithMany()
            .HasForeignKey(j => j.RightTableId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
