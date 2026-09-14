namespace GraphRag.ITSM.Entities;

/// <summary>
/// Base type for every persisted ITSM entity: surrogate key, natural key,
/// audit columns and optimistic-concurrency token.
/// </summary>
public abstract class EntityBase
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>External / business-visible identifier (e.g. "INC0001234", "CI0002211").</summary>
    public string? ExternalId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    /// <summary>Row version for optimistic concurrency (mapped as rowversion / xmin depending on provider).</summary>
    public byte[]? RowVersion { get; set; }
}
