using GraphRag.EFCore.Context;
using GraphRag.EFCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace GraphRag.EFCore.Services
{
    public class GraphEntityService
    {
        private readonly GraphRagDbContext _dbContext;

        public GraphEntityService(GraphRagDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GraphEntity> CreateAsync(GraphEntity entity, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entity);

            _dbContext.GraphEntities.Add(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<GraphEntity?> GetByIdAsync(int id, bool includeAliases = false, CancellationToken cancellationToken = default)
        {
            IQueryable<GraphEntity> query = _dbContext.GraphEntities;

            if (includeAliases)
            {
                query = query.Include(e => e.Aliases);
            }

            return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task<List<GraphEntity>> GetAllAsync(bool includeAliases = false, CancellationToken cancellationToken = default)
        {
            IQueryable<GraphEntity> query = _dbContext.GraphEntities.AsNoTracking();

            if (includeAliases)
            {
                query = query.Include(e => e.Aliases);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<GraphEntity?> UpdateAsync(GraphEntity entity, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entity);

            var existing = await _dbContext.GraphEntities.FirstOrDefaultAsync(e => e.Id == entity.Id, cancellationToken);
            if (existing is null)
            {
                return null;
            }

            existing.Name = entity.Name;
            existing.Type = entity.Type;
            existing.Description = entity.Description;

            await _dbContext.SaveChangesAsync(cancellationToken);
            return existing;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var existing = await _dbContext.GraphEntities.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
            if (existing is null)
            {
                return false;
            }

            _dbContext.GraphEntities.Remove(existing);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}