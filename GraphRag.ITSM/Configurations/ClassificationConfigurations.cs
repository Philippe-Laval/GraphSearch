using GraphRag.ITSM.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphRag.ITSM.Configurations;

internal static class ClassificationLookupExtensions
{
    public static EntityTypeBuilder<T> ConfigureLookup<T>(this EntityTypeBuilder<T> b, string tableName)
        where T : ClassificationLookup
    {
        b.ToTable(tableName);
        b.Property(x => x.Code).HasMaxLength(32).IsRequired();
        b.Property(x => x.Name).HasMaxLength(100).IsRequired();
        b.HasIndex(x => x.Code).IsUnique();
        b.ConfigureEntityBase();
        return b;
    }
}

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> b)
    {
        b.ConfigureLookup("Category");

        b.HasOne(c => c.ParentCategory)
            .WithMany(c => c.Children)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class StatusConfiguration : IEntityTypeConfiguration<Status>
{
    public void Configure(EntityTypeBuilder<Status> b)
    {
        b.ConfigureLookup("Status");
        b.Property(s => s.Kind).HasConversion<string>().HasMaxLength(32);
    }
}

public class ImpactConfiguration : IEntityTypeConfiguration<Impact>
{
    public void Configure(EntityTypeBuilder<Impact> b)
    {
        b.ConfigureLookup("Impact");
        b.Property(s => s.Level).HasConversion<string>().HasMaxLength(16);
    }
}

public class UrgencyConfiguration : IEntityTypeConfiguration<Urgency>
{
    public void Configure(EntityTypeBuilder<Urgency> b)
    {
        b.ConfigureLookup("Urgency");
        b.Property(s => s.Level).HasConversion<string>().HasMaxLength(16);
    }
}

public class PriorityConfiguration : IEntityTypeConfiguration<Priority>
{
    public void Configure(EntityTypeBuilder<Priority> b)
    {
        b.ConfigureLookup("Priority");
        b.Property(s => s.Level).HasConversion<string>().HasMaxLength(16);

        b.HasOne(p => p.DerivedFromImpact).WithMany()
            .HasForeignKey(p => p.DerivedFromImpactId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(p => p.DerivedFromUrgency).WithMany()
            .HasForeignKey(p => p.DerivedFromUrgencyId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class SeverityConfiguration : IEntityTypeConfiguration<Severity>
{
    public void Configure(EntityTypeBuilder<Severity> b)
    {
        b.ConfigureLookup("Severity");
        b.Property(s => s.Level).HasConversion<string>().HasMaxLength(16);
    }
}
