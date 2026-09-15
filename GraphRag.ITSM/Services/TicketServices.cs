using GraphRag.ITSM.Context;
using GraphRag.ITSM.Entities;
using Microsoft.EntityFrameworkCore;

namespace GraphRag.ITSM.Services;

public class IncidentService : EntityService<Incident>
{
    public IncidentService(ItsmDbContext dbContext) : base(dbContext) { }
}

public class ServiceRequestService : EntityService<ServiceRequest>
{
    public ServiceRequestService(ItsmDbContext dbContext) : base(dbContext) { }
}

public class ProblemService : EntityService<Problem>
{
    public ProblemService(ItsmDbContext dbContext) : base(dbContext) { }
}

public class ChangeRequestService : EntityService<ChangeRequest>
{
    public ChangeRequestService(ItsmDbContext dbContext) : base(dbContext) { }
}

public class TicketTaskService : EntityService<TicketTask>
{
    public TicketTaskService(ItsmDbContext dbContext) : base(dbContext) { }
}

/// <summary>Join: Ticket &lt;-&gt; ConfigurationItem (composite key).</summary>
public class TicketAffectedCIService
{
    private readonly ItsmDbContext _db;
    public TicketAffectedCIService(ItsmDbContext db) => _db = db;

    public async Task<IReadOnlyList<TicketAffectedCI>> GetAllAsync(CancellationToken ct = default)
        => await _db.TicketAffectedCIs.AsNoTracking().ToListAsync(ct);

    public async Task<TicketAffectedCI?> GetAsync(Guid ticketId, Guid configurationItemId, CancellationToken ct = default)
        => await _db.TicketAffectedCIs.AsNoTracking()
            .FirstOrDefaultAsync(x => x.TicketId == ticketId && x.ConfigurationItemId == configurationItemId, ct);

    public async Task<TicketAffectedCI> CreateAsync(TicketAffectedCI entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await _db.TicketAffectedCIs.AddAsync(entity, ct);
        await _db.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<TicketAffectedCI?> UpdateAsync(TicketAffectedCI entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var existing = await _db.TicketAffectedCIs
            .FirstOrDefaultAsync(x => x.TicketId == entity.TicketId && x.ConfigurationItemId == entity.ConfigurationItemId, ct);
        if (existing is null) return null;
        existing.IsPrimary = entity.IsPrimary;
        await _db.SaveChangesAsync(ct);
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid ticketId, Guid configurationItemId, CancellationToken ct = default)
    {
        var existing = await _db.TicketAffectedCIs
            .FirstOrDefaultAsync(x => x.TicketId == ticketId && x.ConfigurationItemId == configurationItemId, ct);
        if (existing is null) return false;
        _db.TicketAffectedCIs.Remove(existing);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}

/// <summary>Join: Ticket &lt;-&gt; Service (composite key).</summary>
public class TicketAffectedServiceService
{
    private readonly ItsmDbContext _db;
    public TicketAffectedServiceService(ItsmDbContext db) => _db = db;

    public async Task<IReadOnlyList<TicketAffectedService>> GetAllAsync(CancellationToken ct = default)
        => await _db.TicketAffectedServices.AsNoTracking().ToListAsync(ct);

    public async Task<TicketAffectedService?> GetAsync(Guid ticketId, Guid serviceId, CancellationToken ct = default)
        => await _db.TicketAffectedServices.AsNoTracking()
            .FirstOrDefaultAsync(x => x.TicketId == ticketId && x.ServiceId == serviceId, ct);

    public async Task<TicketAffectedService> CreateAsync(TicketAffectedService entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await _db.TicketAffectedServices.AddAsync(entity, ct);
        await _db.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<TicketAffectedService?> UpdateAsync(TicketAffectedService entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var existing = await _db.TicketAffectedServices
            .FirstOrDefaultAsync(x => x.TicketId == entity.TicketId && x.ServiceId == entity.ServiceId, ct);
        if (existing is null) return null;
        existing.IsPrimary = entity.IsPrimary;
        await _db.SaveChangesAsync(ct);
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid ticketId, Guid serviceId, CancellationToken ct = default)
    {
        var existing = await _db.TicketAffectedServices
            .FirstOrDefaultAsync(x => x.TicketId == ticketId && x.ServiceId == serviceId, ct);
        if (existing is null) return false;
        _db.TicketAffectedServices.Remove(existing);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}

/// <summary>Ticket-to-ticket relations. Has its own single Guid PK but does not inherit EntityBase.</summary>
public class TicketRelationService
{
    private readonly ItsmDbContext _db;
    public TicketRelationService(ItsmDbContext db) => _db = db;

    public async Task<IReadOnlyList<TicketRelation>> GetAllAsync(CancellationToken ct = default)
        => await _db.TicketRelations.AsNoTracking().ToListAsync(ct);

    public async Task<TicketRelation?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.TicketRelations.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<TicketRelation> CreateAsync(TicketRelation entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        if (entity.Id == Guid.Empty) entity.Id = Guid.NewGuid();
        if (entity.CreatedAt == default) entity.CreatedAt = DateTimeOffset.UtcNow;
        await _db.TicketRelations.AddAsync(entity, ct);
        await _db.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<TicketRelation?> UpdateAsync(Guid id, TicketRelation entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var existing = await _db.TicketRelations.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (existing is null) return null;
        existing.SourceTicketId = entity.SourceTicketId;
        existing.TargetTicketId = entity.TargetTicketId;
        existing.RelationKind = entity.RelationKind;
        await _db.SaveChangesAsync(ct);
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var existing = await _db.TicketRelations.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (existing is null) return false;
        _db.TicketRelations.Remove(existing);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
