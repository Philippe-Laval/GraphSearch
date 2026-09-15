using GraphRag.Conceptual.Entities;
using Microsoft.EntityFrameworkCore;

namespace GraphRag.Conceptual.Context;

/// <summary>
/// EF Core 10 DbContext persisting the *conceptual* ontology — the bridge between
/// the natural-language / business vocabulary and the physical database catalog.
///
/// This context is deliberately independent from <c>GraphRag.ITSM</c> (which is the
/// operational data). It stores:
///   - Concepts (Incident, Priority, MTTR, ...)
///   - Terms (synonyms, abbreviations, embeddings) for NL matching
///   - DataAssetRef (physical tables/columns known to the ontology)
///   - ConceptRepresentation (concept ⇄ asset + filter — the actual mapping)
///   - JoinRelationship (preferred join paths, cardinality)
///   - BusinessMetricDefinition + MetricImplementation (per-dialect SQL)
///   - CompetencyQuestion + SemanticQuerySample (for evaluation and few-shot prompting)
///   - Governance: DataClassificationRule, AccessPolicyRule, DataOwnerRecord, QualityConstraint
/// </summary>
public class OntologyDbContext : DbContext
{
    public OntologyDbContext(DbContextOptions<OntologyDbContext> options) : base(options) { }

    public DbSet<Concept> Concepts => Set<Concept>();
    public DbSet<ConceptRelation> ConceptRelations => Set<ConceptRelation>();
    public DbSet<Term> Terms => Set<Term>();
    public DbSet<DataAssetRef> DataAssets => Set<DataAssetRef>();
    public DbSet<ConceptRepresentation> ConceptRepresentations => Set<ConceptRepresentation>();
    public DbSet<JoinRelationship> JoinRelationships => Set<JoinRelationship>();
    public DbSet<BusinessMetricDefinition> BusinessMetricDefinitions => Set<BusinessMetricDefinition>();
    public DbSet<MetricImplementation> MetricImplementations => Set<MetricImplementation>();
    public DbSet<CompetencyQuestion> CompetencyQuestions => Set<CompetencyQuestion>();
    public DbSet<SemanticQuerySample> SemanticQuerySamples => Set<SemanticQuerySample>();
    public DbSet<DataClassificationRule> DataClassificationRules => Set<DataClassificationRule>();
    public DbSet<AccessPolicyRule> AccessPolicyRules => Set<AccessPolicyRule>();
    public DbSet<DataOwnerRecord> DataOwnerRecords => Set<DataOwnerRecord>();
    public DbSet<QualityConstraint> QualityConstraints => Set<QualityConstraint>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OntologyDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override int SaveChanges()
    {
        StampAudit();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        StampAudit();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void StampAudit()
    {
        var now = DateTimeOffset.UtcNow;
        foreach (var entry in ChangeTracker.Entries<ConceptualEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    if (entry.Entity.CreatedAt == default)
                        entry.Entity.CreatedAt = now;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    break;
            }
        }
    }
}
