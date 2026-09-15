using GraphRag.Conceptual.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphRag.Conceptual.Configurations;

public class SemanticQuerySampleConfiguration : ConceptualEntityConfiguration<SemanticQuerySample>
{
    public override void Configure(EntityTypeBuilder<SemanticQuerySample> builder)
    {
        base.Configure(builder);

        builder.ToTable("SemanticQuerySample");
        builder.Property(s => s.NaturalLanguageQuestion).HasMaxLength(1000).IsRequired();
        builder.Property(s => s.Intent).HasConversion<string>().HasMaxLength(24);
        builder.Property(s => s.TargetConceptUri).HasMaxLength(256).IsRequired();
        builder.Property(s => s.SemanticQueryJson).HasColumnType("nvarchar(max)");
        builder.Property(s => s.ExpectedSql).HasColumnType("nvarchar(max)");

        builder.HasIndex(s => s.TargetConceptUri);

        builder.HasOne(s => s.CompetencyQuestion)
            .WithMany(q => q.Samples)
            .HasForeignKey(s => s.CompetencyQuestionId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
