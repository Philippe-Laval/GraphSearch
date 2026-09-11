using GraphRag.EFCore.Context;
using GraphRag.EFCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace GraphRag.EFCore.Services
{
    public class GraphEntityAliasService
    {
        private readonly GraphRagDbContext _dbContext;

        public GraphEntityAliasService(GraphRagDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GraphEntityAlias> CreateAsync(GraphEntityAlias alias, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(alias);

            _dbContext.GraphEntityAliases.Add(alias);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return alias;
        }

        public async Task<GraphEntityAlias?> GetAsync(int entityId, string alias, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrEmpty(alias);

            return await _dbContext.GraphEntityAliases
                .FirstOrDefaultAsync(a => a.EntityId == entityId && a.Alias == alias, cancellationToken);
        }

        public async Task<List<GraphEntityAlias>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.GraphEntityAliases
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<List<GraphEntityAlias>> GetByEntityIdAsync(int entityId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.GraphEntityAliases
                .AsNoTracking()
                .Where(a => a.EntityId == entityId)
                .ToListAsync(cancellationToken);
        }

        public async Task<GraphEntityAlias?> UpdateAsync(int entityId, string currentAlias, string newAlias, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrEmpty(currentAlias);
            ArgumentException.ThrowIfNullOrEmpty(newAlias);

            var existing = await _dbContext.GraphEntityAliases
                .FirstOrDefaultAsync(a => a.EntityId == entityId && a.Alias == currentAlias, cancellationToken);
            if (existing is null)
            {
                return null;
            }

            if (string.Equals(existing.Alias, newAlias, StringComparison.Ordinal))
            {
                return existing;
            }

            // Alias is part of the composite primary key, so remove and re-insert.
            _dbContext.GraphEntityAliases.Remove(existing);
            var updated = new GraphEntityAlias
            {
                EntityId = entityId,
                Alias = newAlias
            };
            _dbContext.GraphEntityAliases.Add(updated);

            await _dbContext.SaveChangesAsync(cancellationToken);
            return updated;
        }

        public async Task<bool> DeleteAsync(int entityId, string alias, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrEmpty(alias);

            var existing = await _dbContext.GraphEntityAliases
                .FirstOrDefaultAsync(a => a.EntityId == entityId && a.Alias == alias, cancellationToken);
            if (existing is null)
            {
                return false;
            }

            _dbContext.GraphEntityAliases.Remove(existing);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}