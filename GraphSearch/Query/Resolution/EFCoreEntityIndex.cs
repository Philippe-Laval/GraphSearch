using GraphRag.EFCore.Context;
using Microsoft.EntityFrameworkCore;

namespace GraphSearch.Library.Query.Resolution
{
    public sealed class EFCoreEntityIndex : IEntityIndex
    {
        private readonly GraphRagDbContext _dbContext;

        public EFCoreEntityIndex(GraphRagDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<EntityCandidate>> SearchAsync(
            string text,
            string? entityType,
            int topK,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(text);

            if (topK <= 0)
                return [];

            var lowered = text.ToLower();
            var prefix = $"{lowered}%";
            var contains = $"%{lowered}%";

            /*
             * This code was transleted from sql found in SqlEntityIndex.cs
             *
            - LEFT JOIN via from a in ... .DefaultIfEmpty() (LINQ pattern EF translates to LEFT JOIN).
            - AsNoTracking() since results are read-only projections (best practice for query-only paths — avoids change-tracker overhead).
            - Case-insensitive matching with .ToLower() (translated to SQL LOWER(...)) and EF.Functions.Like for %prefix% / %contains% patterns (parameterized, safe from injection).
            - Optional type filter short-circuits when entityType == null.
            - CASE-based scoring expressed as chained C# ternaries; EF translates this to a SQL CASE WHEN.
            - Server-side ordering + Take(topK) (OrderByDescending + ThenBy + Take), so paging happens in SQL, not in memory.
            - Anonymous-type projection first, then materialize to EntityCandidate after the query — keeps the SQL projection minimal and lets EF use the record constructor cleanly in memory.
            - Preserved original semantics: because of the LEFT JOIN, an entity can appear multiple times (one row per alias match), matching the raw-SQL behavior. 
             */

            // LEFT JOIN GraphEntity -> GraphEntityAlias, mirroring the raw SQL.
            // AsNoTracking: read-only projection, no change tracking needed.
            var query =
                from e in _dbContext.GraphEntities.AsNoTracking()
                from a in _dbContext.GraphEntityAliases
                    .Where(x => x.EntityId == e.Id)
                    .DefaultIfEmpty()
                where
                    (entityType == null || e.Type == entityType)
                    && (
                        e.Name.ToLower() == lowered
                        || (a != null && a.Alias.ToLower() == lowered)
                        || EF.Functions.Like(e.Name.ToLower(), prefix)
                        || (a != null && EF.Functions.Like(a.Alias.ToLower(), prefix))
                        || EF.Functions.Like(e.Name.ToLower(), contains)
                        || (a != null && EF.Functions.Like(a.Alias.ToLower(), contains))
                    )
                select new
                {
                    e.Id,
                    e.Name,
                    e.Type,
                    Score =
                        e.Name.ToLower() == lowered ? 1.0 :
                        (a != null && a.Alias.ToLower() == lowered) ? 0.95 :
                        EF.Functions.Like(e.Name.ToLower(), prefix) ? 0.80 :
                        (a != null && EF.Functions.Like(a.Alias.ToLower(), prefix)) ? 0.75 :
                        0.50
                };

            var rows = await query
                .OrderByDescending(r => r.Score)
                .ThenBy(r => r.Name)
                .Take(topK)
                .ToListAsync(cancellationToken);

            return rows
                .Select(r => new EntityCandidate(r.Id, r.Name, r.Type, r.Score))
                .ToList();
        }
    }
}
