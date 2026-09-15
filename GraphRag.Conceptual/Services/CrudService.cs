using GraphRag.Conceptual.Context;
using GraphRag.Conceptual.Entities;
using Microsoft.EntityFrameworkCore;

namespace GraphRag.Conceptual.Services;

/// <summary>
/// Generic EF Core implementation of <see cref="ICrudService{T}"/> against
/// <see cref="OntologyDbContext"/>. Derive per-entity services from this class.
/// </summary>
public abstract class CrudService<T> : ICrudService<T> where T : ConceptualEntity
{
    protected readonly OntologyDbContext DbContext;
    protected readonly DbSet<T> Set;

    protected CrudService(OntologyDbContext dbContext)
    {
        DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        Set = DbContext.Set<T>();
    }

    protected virtual IQueryable<T> Query() => Set.AsNoTracking();

    public virtual Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Set.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public virtual Task<T?> GetByUriAsync(string uri, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uri);
        return Set.FirstOrDefaultAsync(e => e.Uri == uri, cancellationToken);
    }

    public virtual async Task<IReadOnlyList<T>> ListAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default)
    {
        if (skip < 0) throw new ArgumentOutOfRangeException(nameof(skip));
        if (take <= 0) throw new ArgumentOutOfRangeException(nameof(take));

        return await Set.AsNoTracking()
            .OrderBy(e => e.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Query().ToListAsync(cancellationToken);
    }

    public virtual async Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await Set.AddAsync(entity, cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public virtual async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        Set.Update(entity);
        await DbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public virtual async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existing = await Set.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (existing is null) return false;
        Set.Remove(existing);
        await DbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public virtual Task<int> CountAsync(CancellationToken cancellationToken = default)
        => Set.CountAsync(cancellationToken);
}
