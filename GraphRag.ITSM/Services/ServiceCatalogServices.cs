using GraphRag.ITSM.Context;
using GraphRag.ITSM.Entities;
using Microsoft.EntityFrameworkCore;

namespace GraphRag.ITSM.Services;

public class BusinessServiceService : EntityService<BusinessService>
{
    public BusinessServiceService(ItsmDbContext dbContext) : base(dbContext) { }
}

public class TechnicalServiceService : EntityService<TechnicalService>
{
    public TechnicalServiceService(ItsmDbContext dbContext) : base(dbContext) { }
}

public class ServiceOfferingService : EntityService<ServiceOffering>
{
    public ServiceOfferingService(ItsmDbContext dbContext) : base(dbContext) { }
}

public class ServiceCatalogItemService : EntityService<ServiceCatalogItem>
{
    public ServiceCatalogItemService(ItsmDbContext dbContext) : base(dbContext) { }
}

/// <summary>Join: Service &lt;-&gt; AssignmentGroup (composite key).</summary>
public class ServiceSupportGroupService
{
    private readonly ItsmDbContext _db;
    public ServiceSupportGroupService(ItsmDbContext db) => _db = db;

    public async Task<IReadOnlyList<ServiceSupportGroup>> GetAllAsync(CancellationToken ct = default)
        => await _db.ServiceSupportGroups.AsNoTracking().ToListAsync(ct);

    public async Task<ServiceSupportGroup?> GetAsync(Guid serviceId, Guid assignmentGroupId, CancellationToken ct = default)
        => await _db.ServiceSupportGroups.AsNoTracking()
            .FirstOrDefaultAsync(s => s.ServiceId == serviceId && s.AssignmentGroupId == assignmentGroupId, ct);

    public async Task<ServiceSupportGroup> CreateAsync(ServiceSupportGroup entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await _db.ServiceSupportGroups.AddAsync(entity, ct);
        await _db.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<ServiceSupportGroup?> UpdateAsync(ServiceSupportGroup entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var existing = await _db.ServiceSupportGroups
            .FirstOrDefaultAsync(s => s.ServiceId == entity.ServiceId && s.AssignmentGroupId == entity.AssignmentGroupId, ct);
        if (existing is null) return null;
        existing.IsPrimary = entity.IsPrimary;
        await _db.SaveChangesAsync(ct);
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid serviceId, Guid assignmentGroupId, CancellationToken ct = default)
    {
        var existing = await _db.ServiceSupportGroups
            .FirstOrDefaultAsync(s => s.ServiceId == serviceId && s.AssignmentGroupId == assignmentGroupId, ct);
        if (existing is null) return false;
        _db.ServiceSupportGroups.Remove(existing);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}

/// <summary>Join: TechnicalService enables BusinessService (composite key).</summary>
public class TechnicalServiceEnablementService
{
    private readonly ItsmDbContext _db;
    public TechnicalServiceEnablementService(ItsmDbContext db) => _db = db;

    public async Task<IReadOnlyList<TechnicalServiceEnablement>> GetAllAsync(CancellationToken ct = default)
        => await _db.TechnicalServiceEnablements.AsNoTracking().ToListAsync(ct);

    public async Task<TechnicalServiceEnablement?> GetAsync(Guid technicalServiceId, Guid businessServiceId, CancellationToken ct = default)
        => await _db.TechnicalServiceEnablements.AsNoTracking()
            .FirstOrDefaultAsync(e => e.TechnicalServiceId == technicalServiceId && e.BusinessServiceId == businessServiceId, ct);

    public async Task<TechnicalServiceEnablement> CreateAsync(TechnicalServiceEnablement entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await _db.TechnicalServiceEnablements.AddAsync(entity, ct);
        await _db.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid technicalServiceId, Guid businessServiceId, CancellationToken ct = default)
    {
        var existing = await _db.TechnicalServiceEnablements
            .FirstOrDefaultAsync(e => e.TechnicalServiceId == technicalServiceId && e.BusinessServiceId == businessServiceId, ct);
        if (existing is null) return false;
        _db.TechnicalServiceEnablements.Remove(existing);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}

/// <summary>Join: BusinessService consumed by Organization (composite key).</summary>
public class BusinessServiceConsumerService
{
    private readonly ItsmDbContext _db;
    public BusinessServiceConsumerService(ItsmDbContext db) => _db = db;

    public async Task<IReadOnlyList<BusinessServiceConsumer>> GetAllAsync(CancellationToken ct = default)
        => await _db.BusinessServiceConsumers.AsNoTracking().ToListAsync(ct);

    public async Task<BusinessServiceConsumer?> GetAsync(Guid businessServiceId, Guid organizationId, CancellationToken ct = default)
        => await _db.BusinessServiceConsumers.AsNoTracking()
            .FirstOrDefaultAsync(e => e.BusinessServiceId == businessServiceId && e.OrganizationId == organizationId, ct);

    public async Task<BusinessServiceConsumer> CreateAsync(BusinessServiceConsumer entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await _db.BusinessServiceConsumers.AddAsync(entity, ct);
        await _db.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid businessServiceId, Guid organizationId, CancellationToken ct = default)
    {
        var existing = await _db.BusinessServiceConsumers
            .FirstOrDefaultAsync(e => e.BusinessServiceId == businessServiceId && e.OrganizationId == organizationId, ct);
        if (existing is null) return false;
        _db.BusinessServiceConsumers.Remove(existing);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
