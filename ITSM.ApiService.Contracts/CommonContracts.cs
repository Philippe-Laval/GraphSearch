using System.ComponentModel.DataAnnotations;

namespace ITSM.ApiService.Contracts;

public abstract record EntityRequestBase
{
    public string? ExternalId { get; init; }

    public string? CreatedBy { get; init; }

    public string? UpdatedBy { get; init; }
}

public abstract record EntityResponseBase(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy);

public static partial class ItsmContractMapper;
