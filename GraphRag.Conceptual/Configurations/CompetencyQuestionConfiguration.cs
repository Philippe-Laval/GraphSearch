using GraphRag.Conceptual.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphRag.Conceptual.Configurations;

public class CompetencyQuestionConfiguration : ConceptualEntityConfiguration<CompetencyQuestion>
{
    public override void Configure(EntityTypeBuilder<CompetencyQuestion> builder)
    {
        base.Configure(builder);

        builder.ToTable("CompetencyQuestion");
        builder.Property(q => q.Domain).HasMaxLength(64).IsRequired();
        builder.Property(q => q.Question).HasMaxLength(1000).IsRequired();
        builder.Property(q => q.Rationale).HasMaxLength(2000);
        builder.Property(q => q.Difficulty).HasConversion<string>().HasMaxLength(16);
        builder.HasIndex(q => q.Domain);
    }
}
