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

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        ConfigureConceptualBase(mb);
        ConfigureConcepts(mb);
        ConfigureTerms(mb);
        ConfigureDataAssets(mb);
        ConfigureRepresentations(mb);
        ConfigureMetrics(mb);
        ConfigureCompetency(mb);
        ConfigureGovernance(mb);
    }

    private static void ConfigureConceptualBase(ModelBuilder mb)
    {
        foreach (var entityType in mb.Model.GetEntityTypes())
        {
            if (typeof(ConceptualEntity).IsAssignableFrom(entityType.ClrType))
            {
                var builder = mb.Entity(entityType.ClrType);
                builder.Property(nameof(ConceptualEntity.Uri)).HasMaxLength(256).IsRequired();
                builder.Property(nameof(ConceptualEntity.PreferredLabel)).HasMaxLength(256);
                builder.Property(nameof(ConceptualEntity.OntologyVersion)).HasMaxLength(32);
                builder.Property(nameof(ConceptualEntity.Source)).HasMaxLength(128);
                builder.Property(nameof(ConceptualEntity.AssertedBy)).HasMaxLength(128);
                builder.Property(nameof(ConceptualEntity.RowVersion)).IsRowVersion();
            }
        }
    }

    private static void ConfigureConcepts(ModelBuilder mb)
    {
        mb.Entity<Concept>(b =>
        {
            b.ToTable("Concept");
            b.HasIndex(c => c.Uri).IsUnique();
            b.HasIndex(c => new { c.Domain, c.Kind });
            b.Property(c => c.Kind).HasConversion<string>().HasMaxLength(32);
            b.Property(c => c.Domain).HasMaxLength(64).IsRequired();
            b.Property(c => c.SynonymsInline).HasMaxLength(2000);

            b.HasOne(c => c.ParentConcept)
                .WithMany(c => c.Children)
                .HasForeignKey(c => c.ParentConceptId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        mb.Entity<ConceptRelation>(b =>
        {
            b.ToTable("ConceptRelation");
            b.Property(r => r.Predicate).HasMaxLength(64).IsRequired();
            b.Property(r => r.Cardinality).HasConversion<string>().HasMaxLength(16);
            b.HasIndex(r => new { r.SourceConceptId, r.Predicate, r.TargetConceptId }).IsUnique();

            b.HasOne(r => r.SourceConcept)
                .WithMany(c => c.OutgoingRelations)
                .HasForeignKey(r => r.SourceConceptId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(r => r.TargetConcept)
                .WithMany(c => c.IncomingRelations)
                .HasForeignKey(r => r.TargetConceptId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureTerms(ModelBuilder mb)
    {
        mb.Entity<Term>(b =>
        {
            b.ToTable("Term");
            b.Property(t => t.SurfaceForm).HasMaxLength(256).IsRequired();
            b.Property(t => t.Kind).HasConversion<string>().HasMaxLength(24);
            b.Property(t => t.Language).HasMaxLength(8);
            b.Property(t => t.Context).HasMaxLength(128);

            b.HasIndex(t => t.SurfaceForm);
            b.HasIndex(t => new { t.SurfaceForm, t.Language, t.Context });

            b.HasOne(t => t.Concept)
                .WithMany(c => c.Terms)
                .HasForeignKey(t => t.ConceptId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureDataAssets(ModelBuilder mb)
    {
        mb.Entity<DataAssetRef>(b =>
        {
            b.ToTable("DataAssetRef");
            b.Property(a => a.Kind).HasConversion<string>().HasMaxLength(16);
            b.Property(a => a.Dialect).HasConversion<string>().HasMaxLength(24);
            b.Property(a => a.FullyQualifiedName).HasMaxLength(512).IsRequired();
            b.Property(a => a.DatabaseName).HasMaxLength(128);
            b.Property(a => a.SchemaName).HasMaxLength(128);
            b.Property(a => a.TableName).HasMaxLength(128);
            b.Property(a => a.ColumnName).HasMaxLength(128);
            b.Property(a => a.DataType).HasMaxLength(64);

            b.HasIndex(a => a.FullyQualifiedName).IsUnique();
            b.HasIndex(a => new { a.DatabaseName, a.SchemaName, a.TableName });

            b.HasOne(a => a.ParentAsset)
                .WithMany(a => a.Children)
                .HasForeignKey(a => a.ParentAssetId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureRepresentations(ModelBuilder mb)
    {
        mb.Entity<ConceptRepresentation>(b =>
        {
            b.ToTable("ConceptRepresentation");
            b.Property(r => r.Kind).HasConversion<string>().HasMaxLength(24);
            b.Property(r => r.FilterExpression).HasMaxLength(2000);
            b.Property(r => r.Rationale).HasMaxLength(1000);

            b.HasIndex(r => new { r.ConceptId, r.AssetId, r.Kind }).IsUnique();

            b.HasOne(r => r.Concept)
                .WithMany(c => c.Representations)
                .HasForeignKey(r => r.ConceptId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(r => r.Asset)
                .WithMany()
                .HasForeignKey(r => r.AssetId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        mb.Entity<JoinRelationship>(b =>
        {
            b.ToTable("JoinRelationship");
            b.Property(j => j.JoinCondition).HasMaxLength(1000).IsRequired();
            b.Property(j => j.Cardinality).HasConversion<string>().HasMaxLength(16);

            b.HasIndex(j => new { j.LeftTableId, j.RightTableId });

            b.HasOne(j => j.LeftTable).WithMany()
                .HasForeignKey(j => j.LeftTableId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(j => j.RightTable).WithMany()
                .HasForeignKey(j => j.RightTableId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureMetrics(ModelBuilder mb)
    {
        mb.Entity<BusinessMetricDefinition>(b =>
        {
            b.ToTable("BusinessMetricDefinition");
            b.Property(m => m.Aggregation).HasConversion<string>().HasMaxLength(24);
            b.Property(m => m.TimeBasis).HasConversion<string>().HasMaxLength(16);
            b.Property(m => m.PreferredDefinition).HasMaxLength(1000);
            b.Property(m => m.Formula).HasMaxLength(1000);
            b.Property(m => m.Grain).HasMaxLength(128);
            b.Property(m => m.QualifyingPopulation).HasMaxLength(128);
            b.Property(m => m.Unit).HasMaxLength(32);

            b.HasIndex(m => m.ConceptId).IsUnique();

            b.HasOne(m => m.Concept).WithMany()
                .HasForeignKey(m => m.ConceptId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(m => m.BusinessCalendarConcept).WithMany()
                .HasForeignKey(m => m.BusinessCalendarConceptId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        mb.Entity<MetricImplementation>(b =>
        {
            b.ToTable("MetricImplementation");
            b.Property(i => i.Dialect).HasConversion<string>().HasMaxLength(24);
            b.Property(i => i.Expression).HasMaxLength(4000).IsRequired();
            b.Property(i => i.RequiredTables).HasMaxLength(1000);

            b.HasIndex(i => new { i.MetricDefinitionId, i.Dialect }).IsUnique();

            b.HasOne(i => i.MetricDefinition)
                .WithMany(m => m.Implementations)
                .HasForeignKey(i => i.MetricDefinitionId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureCompetency(ModelBuilder mb)
    {
        mb.Entity<CompetencyQuestion>(b =>
        {
            b.ToTable("CompetencyQuestion");
            b.Property(q => q.Domain).HasMaxLength(64).IsRequired();
            b.Property(q => q.Question).HasMaxLength(1000).IsRequired();
            b.Property(q => q.Rationale).HasMaxLength(2000);
            b.Property(q => q.Difficulty).HasConversion<string>().HasMaxLength(16);
            b.HasIndex(q => q.Domain);
        });

        mb.Entity<SemanticQuerySample>(b =>
        {
            b.ToTable("SemanticQuerySample");
            b.Property(s => s.NaturalLanguageQuestion).HasMaxLength(1000).IsRequired();
            b.Property(s => s.Intent).HasConversion<string>().HasMaxLength(24);
            b.Property(s => s.TargetConceptUri).HasMaxLength(256).IsRequired();
            b.Property(s => s.SemanticQueryJson).HasColumnType("nvarchar(max)");
            b.Property(s => s.ExpectedSql).HasColumnType("nvarchar(max)");

            b.HasIndex(s => s.TargetConceptUri);

            b.HasOne(s => s.CompetencyQuestion)
                .WithMany(q => q.Samples)
                .HasForeignKey(s => s.CompetencyQuestionId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private static void ConfigureGovernance(ModelBuilder mb)
    {
        mb.Entity<DataClassificationRule>(b =>
        {
            b.ToTable("DataClassificationRule");
            b.Property(r => r.Sensitivity).HasConversion<string>().HasMaxLength(16);
            b.Property(r => r.ComplianceTags).HasMaxLength(256);
            b.HasIndex(r => new { r.AssetId, r.Sensitivity }).IsUnique();
            b.HasOne(r => r.Asset).WithMany()
                .HasForeignKey(r => r.AssetId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        mb.Entity<AccessPolicyRule>(b =>
        {
            b.ToTable("AccessPolicyRule");
            b.Property(r => r.AllowedRolesCsv).HasMaxLength(1000);
            b.Property(r => r.DeniedRolesCsv).HasMaxLength(1000);
            b.Property(r => r.AllowedAggregationsCsv).HasMaxLength(500);
            b.HasIndex(r => r.AssetId);
            b.HasOne(r => r.Asset).WithMany()
                .HasForeignKey(r => r.AssetId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        mb.Entity<DataOwnerRecord>(b =>
        {
            b.ToTable("DataOwnerRecord");
            b.Property(r => r.OwnerTeam).HasMaxLength(128).IsRequired();
            b.Property(r => r.Steward).HasMaxLength(128);
            b.Property(r => r.DataDomain).HasMaxLength(64);
            b.HasIndex(r => r.AssetId).IsUnique();
            b.HasOne(r => r.Asset).WithMany()
                .HasForeignKey(r => r.AssetId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        mb.Entity<QualityConstraint>(b =>
        {
            b.ToTable("QualityConstraint");
            b.Property(r => r.QualityLevel).HasMaxLength(32);
            b.HasIndex(r => r.AssetId).IsUnique();
            b.HasOne(r => r.Asset).WithMany()
                .HasForeignKey(r => r.AssetId)
                .OnDelete(DeleteBehavior.Cascade);
        });
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
