using GraphRag.ITSM.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphRag.ITSM.Configurations;

public class ServiceLevelAgreementConfiguration : IEntityTypeConfiguration<ServiceLevelAgreement>
{
    public void Configure(EntityTypeBuilder<ServiceLevelAgreement> b)
    {
        b.Property(s => s.Name).HasMaxLength(200).IsRequired();
        b.Property(s => s.Kind).HasConversion<string>().HasMaxLength(16);
        b.HasIndex(s => s.Name);

        b.HasOne(s => s.AppliesToPriority).WithMany()
            .HasForeignKey(s => s.AppliesToPriorityId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(s => s.AppliesToService).WithMany()
            .HasForeignKey(s => s.AppliesToServiceId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(s => s.BusinessCalendar).WithMany()
            .HasForeignKey(s => s.BusinessCalendarId).OnDelete(DeleteBehavior.Restrict);

        b.ConfigureEntityBase();
    }
}

public class SlaMeasurementConfiguration : IEntityTypeConfiguration<SlaMeasurement>
{
    public void Configure(EntityTypeBuilder<SlaMeasurement> b)
    {
        b.Property(m => m.BreachStatus).HasConversion<string>().HasMaxLength(16);
        b.HasIndex(m => new { m.TicketId, m.ServiceLevelAgreementId });

        b.HasOne(m => m.Ticket).WithMany(t => t.SlaMeasurements)
            .HasForeignKey(m => m.TicketId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(m => m.ServiceLevelAgreement).WithMany()
            .HasForeignKey(m => m.ServiceLevelAgreementId).OnDelete(DeleteBehavior.Restrict);

        b.ConfigureEntityBase();
    }
}

public class BusinessCalendarConfiguration : IEntityTypeConfiguration<BusinessCalendar>
{
    public void Configure(EntityTypeBuilder<BusinessCalendar> b)
    {
        b.Property(c => c.Name).HasMaxLength(100).IsRequired();
        b.HasIndex(c => c.Name).IsUnique();

        b.ConfigureEntityBase();
    }
}

public class CalendarHolidayConfiguration : IEntityTypeConfiguration<CalendarHoliday>
{
    public void Configure(EntityTypeBuilder<CalendarHoliday> b)
    {
        b.HasKey(h => h.Id);

        b.HasOne(h => h.BusinessCalendar).WithMany(c => c.Holidays)
            .HasForeignKey(h => h.BusinessCalendarId).OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(h => new { h.BusinessCalendarId, h.Date }).IsUnique();
    }
}
