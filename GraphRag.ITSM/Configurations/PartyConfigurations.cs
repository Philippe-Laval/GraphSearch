using GraphRag.ITSM.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphRag.ITSM.Configurations;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> b)
    {
        b.ToTable("Person");
        b.HasDiscriminator<string>("PersonType")
            .HasValue<Person>("User")
            .HasValue<Agent>("Agent");

        b.Property(p => p.FullName).HasMaxLength(200).IsRequired();
        b.Property(p => p.Email).HasMaxLength(320);
        b.HasIndex(p => p.Email);

        b.HasOne(p => p.Organization)
            .WithMany(o => o.Members)
            .HasForeignKey(p => p.OrganizationId)
            .OnDelete(DeleteBehavior.SetNull);

        b.ConfigureEntityBase();
    }
}

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> b)
    {
        b.Property(o => o.Name).HasMaxLength(200).IsRequired();

        b.HasOne(o => o.ParentOrganization)
            .WithMany(o => o.Children)
            .HasForeignKey(o => o.ParentOrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        b.ConfigureEntityBase();
    }
}

public class AssignmentGroupConfiguration : IEntityTypeConfiguration<AssignmentGroup>
{
    public void Configure(EntityTypeBuilder<AssignmentGroup> b)
    {
        b.Property(g => g.Name).HasMaxLength(200).IsRequired();
        b.HasIndex(g => g.Name).IsUnique();

        b.ConfigureEntityBase();
    }
}

public class AgentGroupMembershipConfiguration : IEntityTypeConfiguration<AgentGroupMembership>
{
    public void Configure(EntityTypeBuilder<AgentGroupMembership> b)
    {
        b.HasKey(m => new { m.AgentId, m.AssignmentGroupId, m.AssignedAt });

        b.HasOne(m => m.Agent)
            .WithMany(a => a.GroupMemberships)
            .HasForeignKey(m => m.AgentId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(m => m.AssignmentGroup)
            .WithMany(g => g.Memberships)
            .HasForeignKey(m => m.AssignmentGroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class VendorConfiguration : IEntityTypeConfiguration<Vendor>
{
    public void Configure(EntityTypeBuilder<Vendor> b)
    {
        b.Property(v => v.Name).HasMaxLength(200).IsRequired();

        b.ConfigureEntityBase();
    }
}
