using GraphRag.EFCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace GraphRag.EFCore.Context;

public class GraphRagDbContext : DbContext
{
    public GraphRagDbContext(DbContextOptions<GraphRagDbContext> options)
        : base(options)
    {
    }

    public DbSet<GraphEntity> GraphEntities => Set<GraphEntity>();

    public DbSet<GraphEntityAlias> GraphEntityAliases => Set<GraphEntityAlias>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GraphRagDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
