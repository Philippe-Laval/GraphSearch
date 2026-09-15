using GraphRag.ITSM.Context;
using GraphRag.ITSM.Entities;
using Microsoft.EntityFrameworkCore;

namespace GraphRag.ITSM.Services;

public class ApplicationService : EntityService<Application>
{
    public ApplicationService(ItsmDbContext dbContext) : base(dbContext) { }
}

public class ServerService : EntityService<Server>
{
    public ServerService(ItsmDbContext dbContext) : base(dbContext) { }
}

public class DatabaseCIService : EntityService<DatabaseCI>
{
    public DatabaseCIService(ItsmDbContext dbContext) : base(dbContext) { }
}

public class NetworkDeviceService : EntityService<NetworkDevice>
{
    public NetworkDeviceService(ItsmDbContext dbContext) : base(dbContext) { }
}

public class CloudResourceService : EntityService<CloudResource>
{
    public CloudResourceService(ItsmDbContext dbContext) : base(dbContext) { }
}

/// <summary>Join: CI supports TechnicalService (composite key).</summary>
public class ConfigurationItemSupportService
{
    private readonly ItsmDbContext _db;
    public ConfigurationItemSupportService(ItsmDbContext db) => _db = db;

    public async Task<IReadOnlyList<ConfigurationItemSupport>> GetAllAsync(CancellationToken ct = default)
        => await _db.ConfigurationItemSupports.AsNoTracking().ToListAsync(ct);

    public async Task<ConfigurationItemSupport?> GetAsync(Guid configurationItemId, Guid technicalServiceId, CancellationToken ct = default)
        => await _db.ConfigurationItemSupports.AsNoTracking()
            .FirstOrDefaultAsync(x => x.ConfigurationItemId == configurationItemId && x.TechnicalServiceId == technicalServiceId, ct);

    public async Task<ConfigurationItemSupport> CreateAsync(ConfigurationItemSupport entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await _db.ConfigurationItemSupports.AddAsync(entity, ct);
        await _db.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid configurationItemId, Guid technicalServiceId, CancellationToken ct = default)
    {
        var existing = await _db.ConfigurationItemSupports
            .FirstOrDefaultAsync(x => x.ConfigurationItemId == configurationItemId && x.TechnicalServiceId == technicalServiceId, ct);
        if (existing is null) return false;
        _db.ConfigurationItemSupports.Remove(existing);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}

/// <summary>Join: CI depends on CI (self-referencing, composite key).</summary>
public class ConfigurationItemDependencyService
{
    private readonly ItsmDbContext _db;
    public ConfigurationItemDependencyService(ItsmDbContext db) => _db = db;

    public async Task<IReadOnlyList<ConfigurationItemDependency>> GetAllAsync(CancellationToken ct = default)
        => await _db.ConfigurationItemDependencies.AsNoTracking().ToListAsync(ct);

    public async Task<ConfigurationItemDependency?> GetAsync(Guid sourceId, Guid targetId, CancellationToken ct = default)
        => await _db.ConfigurationItemDependencies.AsNoTracking()
            .FirstOrDefaultAsync(x => x.SourceId == sourceId && x.TargetId == targetId, ct);

    public async Task<ConfigurationItemDependency> CreateAsync(ConfigurationItemDependency entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await _db.ConfigurationItemDependencies.AddAsync(entity, ct);
        await _db.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<ConfigurationItemDependency?> UpdateAsync(ConfigurationItemDependency entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var existing = await _db.ConfigurationItemDependencies
            .FirstOrDefaultAsync(x => x.SourceId == entity.SourceId && x.TargetId == entity.TargetId, ct);
        if (existing is null) return null;
        existing.DependencyType = entity.DependencyType;
        await _db.SaveChangesAsync(ct);
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid sourceId, Guid targetId, CancellationToken ct = default)
    {
        var existing = await _db.ConfigurationItemDependencies
            .FirstOrDefaultAsync(x => x.SourceId == sourceId && x.TargetId == targetId, ct);
        if (existing is null) return false;
        _db.ConfigurationItemDependencies.Remove(existing);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
