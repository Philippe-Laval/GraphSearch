using GraphRag.ITSM.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphRag.ITSM.Configurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> b)
    {
        b.ToTable("Service");
        b.HasDiscriminator<string>("ServiceType")
            .HasValue<BusinessService>("Business")
            .HasValue<TechnicalService>("Technical")
            .HasValue<ServiceOffering>("Offering")
            .HasValue<ServiceCatalogItem>("Catalog");

        b.Property(s => s.Name).HasMaxLength(200).IsRequired();
        b.HasIndex(s => s.Name);

        b.HasOne(s => s.Owner)
            .WithMany()
            .HasForeignKey(s => s.OwnerId)
            .OnDelete(DeleteBehavior.SetNull);

        b.ConfigureEntityBase();
    }
}

public class ServiceOfferingConfiguration : IEntityTypeConfiguration<ServiceOffering>
{
    public void Configure(EntityTypeBuilder<ServiceOffering> b)
    {
        b.HasOne(o => o.BusinessService)
            .WithMany()
            .HasForeignKey(o => o.BusinessServiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ServiceSupportGroupConfiguration : IEntityTypeConfiguration<ServiceSupportGroup>
{
    public void Configure(EntityTypeBuilder<ServiceSupportGroup> b)
    {
        b.HasKey(x => new { x.ServiceId, x.AssignmentGroupId });

        b.HasOne(x => x.Service).WithMany(s => s.SupportGroups)
            .HasForeignKey(x => x.ServiceId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.AssignmentGroup).WithMany(g => g.SupportedServices)
            .HasForeignKey(x => x.AssignmentGroupId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class TechnicalServiceEnablementConfiguration : IEntityTypeConfiguration<TechnicalServiceEnablement>
{
    public void Configure(EntityTypeBuilder<TechnicalServiceEnablement> b)
    {
        b.HasKey(x => new { x.TechnicalServiceId, x.BusinessServiceId });

        b.HasOne(x => x.TechnicalService).WithMany(s => s.Enables)
            .HasForeignKey(x => x.TechnicalServiceId).OnDelete(DeleteBehavior.NoAction);
        b.HasOne(x => x.BusinessService).WithMany(s => s.EnabledBy)
            .HasForeignKey(x => x.BusinessServiceId).OnDelete(DeleteBehavior.NoAction);
    }
}

public class BusinessServiceConsumerConfiguration : IEntityTypeConfiguration<BusinessServiceConsumer>
{
    public void Configure(EntityTypeBuilder<BusinessServiceConsumer> b)
    {
        b.HasKey(x => new { x.BusinessServiceId, x.OrganizationId });

        b.HasOne(x => x.BusinessService).WithMany(s => s.ConsumedBy)
            .HasForeignKey(x => x.BusinessServiceId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Organization).WithMany()
            .HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Cascade);
    }
}
