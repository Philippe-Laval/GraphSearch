using GraphRag.ITSM.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphRag.ITSM.Configurations;

public class ConfigurationItemConfiguration : IEntityTypeConfiguration<ConfigurationItem>
{
    public void Configure(EntityTypeBuilder<ConfigurationItem> b)
    {
        b.ToTable("ConfigurationItem");
        b.HasDiscriminator<string>("CIType")
            .HasValue<Application>("Application")
            .HasValue<Server>("Server")
            .HasValue<DatabaseCI>("Database")
            .HasValue<NetworkDevice>("NetworkDevice")
            .HasValue<CloudResource>("CloudResource");

        b.Property(c => c.Name).HasMaxLength(200).IsRequired();
        b.HasIndex(c => c.Name);
        b.HasIndex(c => c.AssetTag);
        b.Property(c => c.OperationalStatus).HasConversion<string>().HasMaxLength(32);

        b.ConfigureEntityBase();
    }
}

public class ConfigurationItemSupportConfiguration : IEntityTypeConfiguration<ConfigurationItemSupport>
{
    public void Configure(EntityTypeBuilder<ConfigurationItemSupport> b)
    {
        b.HasKey(x => new { x.ConfigurationItemId, x.TechnicalServiceId });

        b.HasOne(x => x.ConfigurationItem).WithMany(c => c.SupportedTechnicalServices)
            .HasForeignKey(x => x.ConfigurationItemId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.TechnicalService).WithMany(s => s.SupportedByCIs)
            .HasForeignKey(x => x.TechnicalServiceId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class ConfigurationItemDependencyConfiguration : IEntityTypeConfiguration<ConfigurationItemDependency>
{
    public void Configure(EntityTypeBuilder<ConfigurationItemDependency> b)
    {
        b.HasKey(x => new { x.SourceId, x.TargetId });

        b.HasOne(x => x.Source).WithMany(c => c.DependsOn)
            .HasForeignKey(x => x.SourceId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Target).WithMany(c => c.DependedOnBy)
            .HasForeignKey(x => x.TargetId).OnDelete(DeleteBehavior.Restrict);
    }
}
