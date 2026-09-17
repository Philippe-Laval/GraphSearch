using System.ComponentModel.DataAnnotations;
using GraphRag.ITSM.Entities;

namespace ITSM.ApiService.Contracts;

public abstract record ClassificationRequestBase : EntityRequestBase
{
    [Required, MaxLength(100)]
    public required string Code { get; init; }

    [Required, MaxLength(200)]
    public required string Name { get; init; }

    public int SortOrder { get; init; }

    public bool IsActive { get; init; } = true;
}

public abstract record ClassificationResponseBase(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    string Code,
    string Name,
    int SortOrder,
    bool IsActive)
    : EntityResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy);

/// <summary>Payload for creating or updating a category.</summary>
public sealed record CategoryRequest : ClassificationRequestBase
{
    public Guid? ParentCategoryId { get; init; }
}

/// <summary>Represents a category returned by the API.</summary>
public sealed record CategoryResponse(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    string Code,
    string Name,
    int SortOrder,
    bool IsActive,
    Guid? ParentCategoryId)
    : ClassificationResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, Code, Name, SortOrder, IsActive);

/// <summary>Payload for creating or updating a status.</summary>
public sealed record StatusRequest : ClassificationRequestBase
{
    public StatusKind Kind { get; init; }

    public bool IsTerminal { get; init; }
}

/// <summary>Represents a status returned by the API.</summary>
public sealed record StatusResponse(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    string Code,
    string Name,
    int SortOrder,
    bool IsActive,
    StatusKind Kind,
    bool IsTerminal)
    : ClassificationResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, Code, Name, SortOrder, IsActive);

/// <summary>Payload for creating or updating an impact.</summary>
public sealed record ImpactRequest : ClassificationRequestBase
{
    public ImpactLevel Level { get; init; }
}

/// <summary>Represents an impact returned by the API.</summary>
public sealed record ImpactResponse(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    string Code,
    string Name,
    int SortOrder,
    bool IsActive,
    ImpactLevel Level)
    : ClassificationResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, Code, Name, SortOrder, IsActive);

/// <summary>Payload for creating or updating an urgency.</summary>
public sealed record UrgencyRequest : ClassificationRequestBase
{
    public UrgencyLevel Level { get; init; }
}

/// <summary>Represents an urgency returned by the API.</summary>
public sealed record UrgencyResponse(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    string Code,
    string Name,
    int SortOrder,
    bool IsActive,
    UrgencyLevel Level)
    : ClassificationResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, Code, Name, SortOrder, IsActive);

/// <summary>Payload for creating or updating a priority.</summary>
public sealed record PriorityRequest : ClassificationRequestBase
{
    public PriorityLevel Level { get; init; }

    public Guid? DerivedFromImpactId { get; init; }

    public Guid? DerivedFromUrgencyId { get; init; }
}

/// <summary>Represents a priority returned by the API.</summary>
public sealed record PriorityResponse(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    string Code,
    string Name,
    int SortOrder,
    bool IsActive,
    PriorityLevel Level,
    Guid? DerivedFromImpactId,
    Guid? DerivedFromUrgencyId)
    : ClassificationResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, Code, Name, SortOrder, IsActive);

/// <summary>Payload for creating or updating a severity.</summary>
public sealed record SeverityRequest : ClassificationRequestBase
{
    public SeverityLevel Level { get; init; }
}

/// <summary>Represents a severity returned by the API.</summary>
public sealed record SeverityResponse(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    string Code,
    string Name,
    int SortOrder,
    bool IsActive,
    SeverityLevel Level)
    : ClassificationResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, Code, Name, SortOrder, IsActive);

internal static partial class ItsmContractMapper
{
    public static Category ToEntity(CategoryRequest request) => new()
    {
        Code = request.Code,
        Name = request.Name,
        SortOrder = request.SortOrder,
        IsActive = request.IsActive,
        ParentCategoryId = request.ParentCategoryId,
        ExternalId = request.ExternalId,
        CreatedBy = request.CreatedBy,
        UpdatedBy = request.UpdatedBy
    };

    public static CategoryResponse ToResponse(Category entity) => new(
        entity.Id,
        entity.ExternalId,
        entity.CreatedAt,
        entity.UpdatedAt,
        entity.CreatedBy,
        entity.UpdatedBy,
        entity.Code,
        entity.Name,
        entity.SortOrder,
        entity.IsActive,
        entity.ParentCategoryId);

    public static Status ToEntity(StatusRequest request) => new()
    {
        Code = request.Code,
        Name = request.Name,
        SortOrder = request.SortOrder,
        IsActive = request.IsActive,
        Kind = request.Kind,
        IsTerminal = request.IsTerminal,
        ExternalId = request.ExternalId,
        CreatedBy = request.CreatedBy,
        UpdatedBy = request.UpdatedBy
    };

    public static StatusResponse ToResponse(Status entity) => new(
        entity.Id,
        entity.ExternalId,
        entity.CreatedAt,
        entity.UpdatedAt,
        entity.CreatedBy,
        entity.UpdatedBy,
        entity.Code,
        entity.Name,
        entity.SortOrder,
        entity.IsActive,
        entity.Kind,
        entity.IsTerminal);

    public static Impact ToEntity(ImpactRequest request) => new()
    {
        Code = request.Code,
        Name = request.Name,
        SortOrder = request.SortOrder,
        IsActive = request.IsActive,
        Level = request.Level,
        ExternalId = request.ExternalId,
        CreatedBy = request.CreatedBy,
        UpdatedBy = request.UpdatedBy
    };

    public static ImpactResponse ToResponse(Impact entity) => new(
        entity.Id,
        entity.ExternalId,
        entity.CreatedAt,
        entity.UpdatedAt,
        entity.CreatedBy,
        entity.UpdatedBy,
        entity.Code,
        entity.Name,
        entity.SortOrder,
        entity.IsActive,
        entity.Level);

    public static Urgency ToEntity(UrgencyRequest request) => new()
    {
        Code = request.Code,
        Name = request.Name,
        SortOrder = request.SortOrder,
        IsActive = request.IsActive,
        Level = request.Level,
        ExternalId = request.ExternalId,
        CreatedBy = request.CreatedBy,
        UpdatedBy = request.UpdatedBy
    };

    public static UrgencyResponse ToResponse(Urgency entity) => new(
        entity.Id,
        entity.ExternalId,
        entity.CreatedAt,
        entity.UpdatedAt,
        entity.CreatedBy,
        entity.UpdatedBy,
        entity.Code,
        entity.Name,
        entity.SortOrder,
        entity.IsActive,
        entity.Level);

    public static Priority ToEntity(PriorityRequest request) => new()
    {
        Code = request.Code,
        Name = request.Name,
        SortOrder = request.SortOrder,
        IsActive = request.IsActive,
        Level = request.Level,
        DerivedFromImpactId = request.DerivedFromImpactId,
        DerivedFromUrgencyId = request.DerivedFromUrgencyId,
        ExternalId = request.ExternalId,
        CreatedBy = request.CreatedBy,
        UpdatedBy = request.UpdatedBy
    };

    public static PriorityResponse ToResponse(Priority entity) => new(
        entity.Id,
        entity.ExternalId,
        entity.CreatedAt,
        entity.UpdatedAt,
        entity.CreatedBy,
        entity.UpdatedBy,
        entity.Code,
        entity.Name,
        entity.SortOrder,
        entity.IsActive,
        entity.Level,
        entity.DerivedFromImpactId,
        entity.DerivedFromUrgencyId);

    public static Severity ToEntity(SeverityRequest request) => new()
    {
        Code = request.Code,
        Name = request.Name,
        SortOrder = request.SortOrder,
        IsActive = request.IsActive,
        Level = request.Level,
        ExternalId = request.ExternalId,
        CreatedBy = request.CreatedBy,
        UpdatedBy = request.UpdatedBy
    };

    public static SeverityResponse ToResponse(Severity entity) => new(
        entity.Id,
        entity.ExternalId,
        entity.CreatedAt,
        entity.UpdatedAt,
        entity.CreatedBy,
        entity.UpdatedBy,
        entity.Code,
        entity.Name,
        entity.SortOrder,
        entity.IsActive,
        entity.Level);
}
