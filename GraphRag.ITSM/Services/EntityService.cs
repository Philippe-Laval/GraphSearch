using GraphRag.ITSM.Context;
using GraphRag.ITSM.Entities;
using Microsoft.EntityFrameworkCore;

namespace GraphRag.ITSM.Services;

/// <summary>
/// Generic CRUD service base for entities deriving from <see cref="EntityBase"/>
/// (single <see cref="Guid"/> primary key + audit columns).
/// </summary>
public abstract class EntityService<TEntity> where TEntity : EntityBase
{
    protected readonly ItsmDbContext DbContext;
    protected readonly DbSet<TEntity> Set;

    protected EntityService(ItsmDbContext dbContext)
    {
        DbContext = dbContext;
        Set = DbContext.Set<TEntity>();
    }

    protected virtual IQueryable<TEntity> Query() => Set.AsNoTracking();

    public virtual async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Query().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public virtual async Task<IReadOnlyList<TEntity>> ListAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default)
    {
        if (skip < 0) throw new ArgumentOutOfRangeException(nameof(skip));
        if (take <= 0) throw new ArgumentOutOfRangeException(nameof(take));

        return await Set.AsNoTracking()
            .OrderBy(e => e.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Query().ToListAsync(cancellationToken);
    }

    public virtual async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        if (entity.Id == Guid.Empty)
        {
            entity.Id = Guid.NewGuid();
        }

        entity.CreatedAt = DateTimeOffset.UtcNow;
        entity.UpdatedAt = null;

        await Set.AddAsync(entity, cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public virtual async Task<TEntity?> UpdateAsync(Guid id, TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var existing = await Set.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        var createdAt = existing.CreatedAt;
        var createdBy = existing.CreatedBy;

        DbContext.Entry(existing).CurrentValues.SetValues(entity);

        existing.Id = id;
        existing.CreatedAt = createdAt;
        existing.CreatedBy = createdBy;
        existing.UpdatedAt = DateTimeOffset.UtcNow;

        await DbContext.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public virtual async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existing = await Set.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        Set.Remove(existing);
        await DbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public virtual Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
         return Set.CountAsync(cancellationToken);
    }
}
