using GraphRag.ITSM.Context;
using GraphRag.ITSM.Entities;
using Microsoft.EntityFrameworkCore;

namespace GraphRag.ITSM.Services;

public class ServiceLevelAgreementService : EntityService<ServiceLevelAgreement>
{
    public ServiceLevelAgreementService(ItsmDbContext dbContext) : base(dbContext) { }
}

public class SlaMeasurementService : EntityService<SlaMeasurement>
{
    public SlaMeasurementService(ItsmDbContext dbContext) : base(dbContext) { }
}

public class BusinessCalendarService : EntityService<BusinessCalendar>
{
    public BusinessCalendarService(ItsmDbContext dbContext) : base(dbContext) { }
}

/// <summary>Calendar holiday. Has a single Guid PK but does not inherit EntityBase.</summary>
public class CalendarHolidayService
{
    private readonly ItsmDbContext _db;
    public CalendarHolidayService(ItsmDbContext db) => _db = db;

    public async Task<IReadOnlyList<CalendarHoliday>> GetAllAsync(CancellationToken ct = default)
        => await _db.CalendarHolidays.AsNoTracking().ToListAsync(ct);

    public async Task<CalendarHoliday?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.CalendarHolidays.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<CalendarHoliday> CreateAsync(CalendarHoliday entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        if (entity.Id == Guid.Empty) entity.Id = Guid.NewGuid();
        await _db.CalendarHolidays.AddAsync(entity, ct);
        await _db.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<CalendarHoliday?> UpdateAsync(Guid id, CalendarHoliday entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var existing = await _db.CalendarHolidays.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (existing is null) return null;
        existing.BusinessCalendarId = entity.BusinessCalendarId;
        existing.Date = entity.Date;
        existing.Name = entity.Name;
        await _db.SaveChangesAsync(ct);
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var existing = await _db.CalendarHolidays.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (existing is null) return false;
        _db.CalendarHolidays.Remove(existing);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
