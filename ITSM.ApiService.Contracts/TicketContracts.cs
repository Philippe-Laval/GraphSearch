using System.ComponentModel.DataAnnotations;
using GraphRag.ITSM.Entities;

namespace ITSM.ApiService.Contracts;

public abstract record TicketRequestBase : EntityRequestBase
{
    [Required, MaxLength(100)]
    public required string Number { get; init; }

    [Required, MaxLength(500)]
    public required string ShortDescription { get; init; }

    public string? Description { get; init; }

    public Guid? RequesterId { get; init; }

    public Guid? AffectedUserId { get; init; }

    public Guid? AssignedAgentId { get; init; }

    public Guid? AssignedGroupId { get; init; }

    public Guid? ResolverId { get; init; }

    public Guid? ResolutionGroupId { get; init; }

    public Guid CurrentStatusId { get; init; }

    public Guid? PriorityId { get; init; }

    public Guid? ImpactId { get; init; }

    public Guid? UrgencyId { get; init; }

    public Guid? SeverityId { get; init; }

    public Guid? CategoryId { get; init; }

    public Guid? SubcategoryId { get; init; }

    public DateTimeOffset OpenedAt { get; init; }

    public DateTimeOffset? AcknowledgedAt { get; init; }

    public DateTimeOffset? AssignedAt { get; init; }

    public DateTimeOffset? FirstResponseAt { get; init; }

    public DateTimeOffset? PendingAt { get; init; }

    public DateTimeOffset? ResolvedAt { get; init; }

    public DateTimeOffset? ClosedAt { get; init; }

    public DateTimeOffset? DueAt { get; init; }

    public DateTimeOffset? SlaResponseDeadline { get; init; }

    public DateTimeOffset? SlaResolutionDeadline { get; init; }

    public int ReopenCount { get; init; }

    public bool WasEscalated { get; init; }
}

public abstract record TicketResponseBase(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    string Number,
    string ShortDescription,
    string? Description,
    Guid? RequesterId,
    Guid? AffectedUserId,
    Guid? AssignedAgentId,
    Guid? AssignedGroupId,
    Guid? ResolverId,
    Guid? ResolutionGroupId,
    Guid CurrentStatusId,
    Guid? PriorityId,
    Guid? ImpactId,
    Guid? UrgencyId,
    Guid? SeverityId,
    Guid? CategoryId,
    Guid? SubcategoryId,
    DateTimeOffset OpenedAt,
    DateTimeOffset? AcknowledgedAt,
    DateTimeOffset? AssignedAt,
    DateTimeOffset? FirstResponseAt,
    DateTimeOffset? PendingAt,
    DateTimeOffset? ResolvedAt,
    DateTimeOffset? ClosedAt,
    DateTimeOffset? DueAt,
    DateTimeOffset? SlaResponseDeadline,
    DateTimeOffset? SlaResolutionDeadline,
    int ReopenCount,
    bool WasEscalated)
    : EntityResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy);

/// <summary>Payload for creating or updating an incident.</summary>
public sealed record IncidentRequest : TicketRequestBase
{
    public Guid? CausedByProblemId { get; init; }

    public Guid? ResolvedByChangeId { get; init; }
}

/// <summary>Represents an incident returned by the API.</summary>
public sealed record IncidentResponse(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    string Number,
    string ShortDescription,
    string? Description,
    Guid? RequesterId,
    Guid? AffectedUserId,
    Guid? AssignedAgentId,
    Guid? AssignedGroupId,
    Guid? ResolverId,
    Guid? ResolutionGroupId,
    Guid CurrentStatusId,
    Guid? PriorityId,
    Guid? ImpactId,
    Guid? UrgencyId,
    Guid? SeverityId,
    Guid? CategoryId,
    Guid? SubcategoryId,
    DateTimeOffset OpenedAt,
    DateTimeOffset? AcknowledgedAt,
    DateTimeOffset? AssignedAt,
    DateTimeOffset? FirstResponseAt,
    DateTimeOffset? PendingAt,
    DateTimeOffset? ResolvedAt,
    DateTimeOffset? ClosedAt,
    DateTimeOffset? DueAt,
    DateTimeOffset? SlaResponseDeadline,
    DateTimeOffset? SlaResolutionDeadline,
    int ReopenCount,
    bool WasEscalated,
    Guid? CausedByProblemId,
    Guid? ResolvedByChangeId)
    : TicketResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, Number, ShortDescription, Description, RequesterId, AffectedUserId, AssignedAgentId, AssignedGroupId, ResolverId, ResolutionGroupId, CurrentStatusId, PriorityId, ImpactId, UrgencyId, SeverityId, CategoryId, SubcategoryId, OpenedAt, AcknowledgedAt, AssignedAt, FirstResponseAt, PendingAt, ResolvedAt, ClosedAt, DueAt, SlaResponseDeadline, SlaResolutionDeadline, ReopenCount, WasEscalated);

/// <summary>Payload for creating or updating a service request.</summary>
public sealed record ServiceRequestRequest : TicketRequestBase
{
    public Guid? RequestedCatalogItemId { get; init; }
}

/// <summary>Represents a service request returned by the API.</summary>
public sealed record ServiceRequestResponse(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    string Number,
    string ShortDescription,
    string? Description,
    Guid? RequesterId,
    Guid? AffectedUserId,
    Guid? AssignedAgentId,
    Guid? AssignedGroupId,
    Guid? ResolverId,
    Guid? ResolutionGroupId,
    Guid CurrentStatusId,
    Guid? PriorityId,
    Guid? ImpactId,
    Guid? UrgencyId,
    Guid? SeverityId,
    Guid? CategoryId,
    Guid? SubcategoryId,
    DateTimeOffset OpenedAt,
    DateTimeOffset? AcknowledgedAt,
    DateTimeOffset? AssignedAt,
    DateTimeOffset? FirstResponseAt,
    DateTimeOffset? PendingAt,
    DateTimeOffset? ResolvedAt,
    DateTimeOffset? ClosedAt,
    DateTimeOffset? DueAt,
    DateTimeOffset? SlaResponseDeadline,
    DateTimeOffset? SlaResolutionDeadline,
    int ReopenCount,
    bool WasEscalated,
    Guid? RequestedCatalogItemId)
    : TicketResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, Number, ShortDescription, Description, RequesterId, AffectedUserId, AssignedAgentId, AssignedGroupId, ResolverId, ResolutionGroupId, CurrentStatusId, PriorityId, ImpactId, UrgencyId, SeverityId, CategoryId, SubcategoryId, OpenedAt, AcknowledgedAt, AssignedAt, FirstResponseAt, PendingAt, ResolvedAt, ClosedAt, DueAt, SlaResponseDeadline, SlaResolutionDeadline, ReopenCount, WasEscalated);

/// <summary>Payload for creating or updating a problem.</summary>
public sealed record ProblemRequest : TicketRequestBase
{
    public string? KnownErrorArticleRef { get; init; }

    public string? RootCause { get; init; }

    public string? Workaround { get; init; }
}

/// <summary>Represents a problem returned by the API.</summary>
public sealed record ProblemResponse(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    string Number,
    string ShortDescription,
    string? Description,
    Guid? RequesterId,
    Guid? AffectedUserId,
    Guid? AssignedAgentId,
    Guid? AssignedGroupId,
    Guid? ResolverId,
    Guid? ResolutionGroupId,
    Guid CurrentStatusId,
    Guid? PriorityId,
    Guid? ImpactId,
    Guid? UrgencyId,
    Guid? SeverityId,
    Guid? CategoryId,
    Guid? SubcategoryId,
    DateTimeOffset OpenedAt,
    DateTimeOffset? AcknowledgedAt,
    DateTimeOffset? AssignedAt,
    DateTimeOffset? FirstResponseAt,
    DateTimeOffset? PendingAt,
    DateTimeOffset? ResolvedAt,
    DateTimeOffset? ClosedAt,
    DateTimeOffset? DueAt,
    DateTimeOffset? SlaResponseDeadline,
    DateTimeOffset? SlaResolutionDeadline,
    int ReopenCount,
    bool WasEscalated,
    string? KnownErrorArticleRef,
    string? RootCause,
    string? Workaround)
    : TicketResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, Number, ShortDescription, Description, RequesterId, AffectedUserId, AssignedAgentId, AssignedGroupId, ResolverId, ResolutionGroupId, CurrentStatusId, PriorityId, ImpactId, UrgencyId, SeverityId, CategoryId, SubcategoryId, OpenedAt, AcknowledgedAt, AssignedAt, FirstResponseAt, PendingAt, ResolvedAt, ClosedAt, DueAt, SlaResponseDeadline, SlaResolutionDeadline, ReopenCount, WasEscalated);

/// <summary>Payload for creating or updating a change request.</summary>
public sealed record ChangeRequestRequest : TicketRequestBase
{
    public ChangeRiskLevel RiskLevel { get; init; }

    public ChangeType ChangeType { get; init; }

    public DateTimeOffset? PlannedStart { get; init; }

    public DateTimeOffset? PlannedEnd { get; init; }

    public DateTimeOffset? ActualStart { get; init; }

    public DateTimeOffset? ActualEnd { get; init; }
}

/// <summary>Represents a change request returned by the API.</summary>
public sealed record ChangeRequestResponse(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    string Number,
    string ShortDescription,
    string? Description,
    Guid? RequesterId,
    Guid? AffectedUserId,
    Guid? AssignedAgentId,
    Guid? AssignedGroupId,
    Guid? ResolverId,
    Guid? ResolutionGroupId,
    Guid CurrentStatusId,
    Guid? PriorityId,
    Guid? ImpactId,
    Guid? UrgencyId,
    Guid? SeverityId,
    Guid? CategoryId,
    Guid? SubcategoryId,
    DateTimeOffset OpenedAt,
    DateTimeOffset? AcknowledgedAt,
    DateTimeOffset? AssignedAt,
    DateTimeOffset? FirstResponseAt,
    DateTimeOffset? PendingAt,
    DateTimeOffset? ResolvedAt,
    DateTimeOffset? ClosedAt,
    DateTimeOffset? DueAt,
    DateTimeOffset? SlaResponseDeadline,
    DateTimeOffset? SlaResolutionDeadline,
    int ReopenCount,
    bool WasEscalated,
    ChangeRiskLevel RiskLevel,
    ChangeType ChangeType,
    DateTimeOffset? PlannedStart,
    DateTimeOffset? PlannedEnd,
    DateTimeOffset? ActualStart,
    DateTimeOffset? ActualEnd)
    : TicketResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, Number, ShortDescription, Description, RequesterId, AffectedUserId, AssignedAgentId, AssignedGroupId, ResolverId, ResolutionGroupId, CurrentStatusId, PriorityId, ImpactId, UrgencyId, SeverityId, CategoryId, SubcategoryId, OpenedAt, AcknowledgedAt, AssignedAt, FirstResponseAt, PendingAt, ResolvedAt, ClosedAt, DueAt, SlaResponseDeadline, SlaResolutionDeadline, ReopenCount, WasEscalated);

/// <summary>Payload for creating or updating a ticket task.</summary>
public sealed record TicketTaskRequest : TicketRequestBase
{
    public Guid? ParentTicketId { get; init; }
}

/// <summary>Represents a ticket task returned by the API.</summary>
public sealed record TicketTaskResponse(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    string Number,
    string ShortDescription,
    string? Description,
    Guid? RequesterId,
    Guid? AffectedUserId,
    Guid? AssignedAgentId,
    Guid? AssignedGroupId,
    Guid? ResolverId,
    Guid? ResolutionGroupId,
    Guid CurrentStatusId,
    Guid? PriorityId,
    Guid? ImpactId,
    Guid? UrgencyId,
    Guid? SeverityId,
    Guid? CategoryId,
    Guid? SubcategoryId,
    DateTimeOffset OpenedAt,
    DateTimeOffset? AcknowledgedAt,
    DateTimeOffset? AssignedAt,
    DateTimeOffset? FirstResponseAt,
    DateTimeOffset? PendingAt,
    DateTimeOffset? ResolvedAt,
    DateTimeOffset? ClosedAt,
    DateTimeOffset? DueAt,
    DateTimeOffset? SlaResponseDeadline,
    DateTimeOffset? SlaResolutionDeadline,
    int ReopenCount,
    bool WasEscalated,
    Guid? ParentTicketId)
    : TicketResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, Number, ShortDescription, Description, RequesterId, AffectedUserId, AssignedAgentId, AssignedGroupId, ResolverId, ResolutionGroupId, CurrentStatusId, PriorityId, ImpactId, UrgencyId, SeverityId, CategoryId, SubcategoryId, OpenedAt, AcknowledgedAt, AssignedAt, FirstResponseAt, PendingAt, ResolvedAt, ClosedAt, DueAt, SlaResponseDeadline, SlaResolutionDeadline, ReopenCount, WasEscalated);

/// <summary>Payload for creating or updating a ticket-to-configuration-item link.</summary>
public sealed record TicketAffectedCIRequest
{
    public Guid TicketId { get; init; }

    public Guid ConfigurationItemId { get; init; }

    public bool IsPrimary { get; init; }
}

/// <summary>Represents a ticket-to-configuration-item link returned by the API.</summary>
public sealed record TicketAffectedCIResponse(Guid TicketId, Guid ConfigurationItemId, bool IsPrimary);

/// <summary>Payload for creating or updating a ticket-to-service link.</summary>
public sealed record TicketAffectedServiceRequest
{
    public Guid TicketId { get; init; }

    public Guid ServiceId { get; init; }

    public bool IsPrimary { get; init; }
}

/// <summary>Represents a ticket-to-service link returned by the API.</summary>
public sealed record TicketAffectedServiceResponse(Guid TicketId, Guid ServiceId, bool IsPrimary);

/// <summary>Payload for creating or updating a ticket relation.</summary>
public sealed record TicketRelationRequest
{
    public Guid SourceTicketId { get; init; }

    public Guid TargetTicketId { get; init; }

    public TicketRelationKind RelationKind { get; init; }

    public DateTimeOffset CreatedAt { get; init; }
}

/// <summary>Represents a ticket relation returned by the API.</summary>
public sealed record TicketRelationResponse(
    Guid Id,
    Guid SourceTicketId,
    Guid TargetTicketId,
    TicketRelationKind RelationKind,
    DateTimeOffset CreatedAt);

public static partial class ItsmContractMapper
{
    public static Incident ToEntity(IncidentRequest request) => new()
    {
        Number = request.Number,
        ShortDescription = request.ShortDescription,
        Description = request.Description,
        RequesterId = request.RequesterId,
        AffectedUserId = request.AffectedUserId,
        AssignedAgentId = request.AssignedAgentId,
        AssignedGroupId = request.AssignedGroupId,
        ResolverId = request.ResolverId,
        ResolutionGroupId = request.ResolutionGroupId,
        CurrentStatusId = request.CurrentStatusId,
        PriorityId = request.PriorityId,
        ImpactId = request.ImpactId,
        UrgencyId = request.UrgencyId,
        SeverityId = request.SeverityId,
        CategoryId = request.CategoryId,
        SubcategoryId = request.SubcategoryId,
        OpenedAt = request.OpenedAt,
        AcknowledgedAt = request.AcknowledgedAt,
        AssignedAt = request.AssignedAt,
        FirstResponseAt = request.FirstResponseAt,
        PendingAt = request.PendingAt,
        ResolvedAt = request.ResolvedAt,
        ClosedAt = request.ClosedAt,
        DueAt = request.DueAt,
        SlaResponseDeadline = request.SlaResponseDeadline,
        SlaResolutionDeadline = request.SlaResolutionDeadline,
        ReopenCount = request.ReopenCount,
        WasEscalated = request.WasEscalated,
        CausedByProblemId = request.CausedByProblemId,
        ResolvedByChangeId = request.ResolvedByChangeId,
        ExternalId = request.ExternalId,
        CreatedBy = request.CreatedBy,
        UpdatedBy = request.UpdatedBy
    };

    public static IncidentResponse ToResponse(Incident entity) => new(
        entity.Id,
        entity.ExternalId,
        entity.CreatedAt,
        entity.UpdatedAt,
        entity.CreatedBy,
        entity.UpdatedBy,
        entity.Number,
        entity.ShortDescription,
        entity.Description,
        entity.RequesterId,
        entity.AffectedUserId,
        entity.AssignedAgentId,
        entity.AssignedGroupId,
        entity.ResolverId,
        entity.ResolutionGroupId,
        entity.CurrentStatusId,
        entity.PriorityId,
        entity.ImpactId,
        entity.UrgencyId,
        entity.SeverityId,
        entity.CategoryId,
        entity.SubcategoryId,
        entity.OpenedAt,
        entity.AcknowledgedAt,
        entity.AssignedAt,
        entity.FirstResponseAt,
        entity.PendingAt,
        entity.ResolvedAt,
        entity.ClosedAt,
        entity.DueAt,
        entity.SlaResponseDeadline,
        entity.SlaResolutionDeadline,
        entity.ReopenCount,
        entity.WasEscalated,
        entity.CausedByProblemId,
        entity.ResolvedByChangeId);

    public static ServiceRequest ToEntity(ServiceRequestRequest request) => new()
    {
        Number = request.Number,
        ShortDescription = request.ShortDescription,
        Description = request.Description,
        RequesterId = request.RequesterId,
        AffectedUserId = request.AffectedUserId,
        AssignedAgentId = request.AssignedAgentId,
        AssignedGroupId = request.AssignedGroupId,
        ResolverId = request.ResolverId,
        ResolutionGroupId = request.ResolutionGroupId,
        CurrentStatusId = request.CurrentStatusId,
        PriorityId = request.PriorityId,
        ImpactId = request.ImpactId,
        UrgencyId = request.UrgencyId,
        SeverityId = request.SeverityId,
        CategoryId = request.CategoryId,
        SubcategoryId = request.SubcategoryId,
        OpenedAt = request.OpenedAt,
        AcknowledgedAt = request.AcknowledgedAt,
        AssignedAt = request.AssignedAt,
        FirstResponseAt = request.FirstResponseAt,
        PendingAt = request.PendingAt,
        ResolvedAt = request.ResolvedAt,
        ClosedAt = request.ClosedAt,
        DueAt = request.DueAt,
        SlaResponseDeadline = request.SlaResponseDeadline,
        SlaResolutionDeadline = request.SlaResolutionDeadline,
        ReopenCount = request.ReopenCount,
        WasEscalated = request.WasEscalated,
        RequestedCatalogItemId = request.RequestedCatalogItemId,
        ExternalId = request.ExternalId,
        CreatedBy = request.CreatedBy,
        UpdatedBy = request.UpdatedBy
    };

    public static ServiceRequestResponse ToResponse(ServiceRequest entity) => new(
        entity.Id,
        entity.ExternalId,
        entity.CreatedAt,
        entity.UpdatedAt,
        entity.CreatedBy,
        entity.UpdatedBy,
        entity.Number,
        entity.ShortDescription,
        entity.Description,
        entity.RequesterId,
        entity.AffectedUserId,
        entity.AssignedAgentId,
        entity.AssignedGroupId,
        entity.ResolverId,
        entity.ResolutionGroupId,
        entity.CurrentStatusId,
        entity.PriorityId,
        entity.ImpactId,
        entity.UrgencyId,
        entity.SeverityId,
        entity.CategoryId,
        entity.SubcategoryId,
        entity.OpenedAt,
        entity.AcknowledgedAt,
        entity.AssignedAt,
        entity.FirstResponseAt,
        entity.PendingAt,
        entity.ResolvedAt,
        entity.ClosedAt,
        entity.DueAt,
        entity.SlaResponseDeadline,
        entity.SlaResolutionDeadline,
        entity.ReopenCount,
        entity.WasEscalated,
        entity.RequestedCatalogItemId);

    public static Problem ToEntity(ProblemRequest request) => new()
    {
        Number = request.Number,
        ShortDescription = request.ShortDescription,
        Description = request.Description,
        RequesterId = request.RequesterId,
        AffectedUserId = request.AffectedUserId,
        AssignedAgentId = request.AssignedAgentId,
        AssignedGroupId = request.AssignedGroupId,
        ResolverId = request.ResolverId,
        ResolutionGroupId = request.ResolutionGroupId,
        CurrentStatusId = request.CurrentStatusId,
        PriorityId = request.PriorityId,
        ImpactId = request.ImpactId,
        UrgencyId = request.UrgencyId,
        SeverityId = request.SeverityId,
        CategoryId = request.CategoryId,
        SubcategoryId = request.SubcategoryId,
        OpenedAt = request.OpenedAt,
        AcknowledgedAt = request.AcknowledgedAt,
        AssignedAt = request.AssignedAt,
        FirstResponseAt = request.FirstResponseAt,
        PendingAt = request.PendingAt,
        ResolvedAt = request.ResolvedAt,
        ClosedAt = request.ClosedAt,
        DueAt = request.DueAt,
        SlaResponseDeadline = request.SlaResponseDeadline,
        SlaResolutionDeadline = request.SlaResolutionDeadline,
        ReopenCount = request.ReopenCount,
        WasEscalated = request.WasEscalated,
        KnownErrorArticleRef = request.KnownErrorArticleRef,
        RootCause = request.RootCause,
        Workaround = request.Workaround,
        ExternalId = request.ExternalId,
        CreatedBy = request.CreatedBy,
        UpdatedBy = request.UpdatedBy
    };

    public static ProblemResponse ToResponse(Problem entity) => new(
        entity.Id,
        entity.ExternalId,
        entity.CreatedAt,
        entity.UpdatedAt,
        entity.CreatedBy,
        entity.UpdatedBy,
        entity.Number,
        entity.ShortDescription,
        entity.Description,
        entity.RequesterId,
        entity.AffectedUserId,
        entity.AssignedAgentId,
        entity.AssignedGroupId,
        entity.ResolverId,
        entity.ResolutionGroupId,
        entity.CurrentStatusId,
        entity.PriorityId,
        entity.ImpactId,
        entity.UrgencyId,
        entity.SeverityId,
        entity.CategoryId,
        entity.SubcategoryId,
        entity.OpenedAt,
        entity.AcknowledgedAt,
        entity.AssignedAt,
        entity.FirstResponseAt,
        entity.PendingAt,
        entity.ResolvedAt,
        entity.ClosedAt,
        entity.DueAt,
        entity.SlaResponseDeadline,
        entity.SlaResolutionDeadline,
        entity.ReopenCount,
        entity.WasEscalated,
        entity.KnownErrorArticleRef,
        entity.RootCause,
        entity.Workaround);

    public static ChangeRequest ToEntity(ChangeRequestRequest request) => new()
    {
        Number = request.Number,
        ShortDescription = request.ShortDescription,
        Description = request.Description,
        RequesterId = request.RequesterId,
        AffectedUserId = request.AffectedUserId,
        AssignedAgentId = request.AssignedAgentId,
        AssignedGroupId = request.AssignedGroupId,
        ResolverId = request.ResolverId,
        ResolutionGroupId = request.ResolutionGroupId,
        CurrentStatusId = request.CurrentStatusId,
        PriorityId = request.PriorityId,
        ImpactId = request.ImpactId,
        UrgencyId = request.UrgencyId,
        SeverityId = request.SeverityId,
        CategoryId = request.CategoryId,
        SubcategoryId = request.SubcategoryId,
        OpenedAt = request.OpenedAt,
        AcknowledgedAt = request.AcknowledgedAt,
        AssignedAt = request.AssignedAt,
        FirstResponseAt = request.FirstResponseAt,
        PendingAt = request.PendingAt,
        ResolvedAt = request.ResolvedAt,
        ClosedAt = request.ClosedAt,
        DueAt = request.DueAt,
        SlaResponseDeadline = request.SlaResponseDeadline,
        SlaResolutionDeadline = request.SlaResolutionDeadline,
        ReopenCount = request.ReopenCount,
        WasEscalated = request.WasEscalated,
        RiskLevel = request.RiskLevel,
        ChangeType = request.ChangeType,
        PlannedStart = request.PlannedStart,
        PlannedEnd = request.PlannedEnd,
        ActualStart = request.ActualStart,
        ActualEnd = request.ActualEnd,
        ExternalId = request.ExternalId,
        CreatedBy = request.CreatedBy,
        UpdatedBy = request.UpdatedBy
    };

    public static ChangeRequestResponse ToResponse(ChangeRequest entity) => new(
        entity.Id,
        entity.ExternalId,
        entity.CreatedAt,
        entity.UpdatedAt,
        entity.CreatedBy,
        entity.UpdatedBy,
        entity.Number,
        entity.ShortDescription,
        entity.Description,
        entity.RequesterId,
        entity.AffectedUserId,
        entity.AssignedAgentId,
        entity.AssignedGroupId,
        entity.ResolverId,
        entity.ResolutionGroupId,
        entity.CurrentStatusId,
        entity.PriorityId,
        entity.ImpactId,
        entity.UrgencyId,
        entity.SeverityId,
        entity.CategoryId,
        entity.SubcategoryId,
        entity.OpenedAt,
        entity.AcknowledgedAt,
        entity.AssignedAt,
        entity.FirstResponseAt,
        entity.PendingAt,
        entity.ResolvedAt,
        entity.ClosedAt,
        entity.DueAt,
        entity.SlaResponseDeadline,
        entity.SlaResolutionDeadline,
        entity.ReopenCount,
        entity.WasEscalated,
        entity.RiskLevel,
        entity.ChangeType,
        entity.PlannedStart,
        entity.PlannedEnd,
        entity.ActualStart,
        entity.ActualEnd);

    public static TicketTask ToEntity(TicketTaskRequest request) => new()
    {
        Number = request.Number,
        ShortDescription = request.ShortDescription,
        Description = request.Description,
        RequesterId = request.RequesterId,
        AffectedUserId = request.AffectedUserId,
        AssignedAgentId = request.AssignedAgentId,
        AssignedGroupId = request.AssignedGroupId,
        ResolverId = request.ResolverId,
        ResolutionGroupId = request.ResolutionGroupId,
        CurrentStatusId = request.CurrentStatusId,
        PriorityId = request.PriorityId,
        ImpactId = request.ImpactId,
        UrgencyId = request.UrgencyId,
        SeverityId = request.SeverityId,
        CategoryId = request.CategoryId,
        SubcategoryId = request.SubcategoryId,
        OpenedAt = request.OpenedAt,
        AcknowledgedAt = request.AcknowledgedAt,
        AssignedAt = request.AssignedAt,
        FirstResponseAt = request.FirstResponseAt,
        PendingAt = request.PendingAt,
        ResolvedAt = request.ResolvedAt,
        ClosedAt = request.ClosedAt,
        DueAt = request.DueAt,
        SlaResponseDeadline = request.SlaResponseDeadline,
        SlaResolutionDeadline = request.SlaResolutionDeadline,
        ReopenCount = request.ReopenCount,
        WasEscalated = request.WasEscalated,
        ParentTicketId = request.ParentTicketId,
        ExternalId = request.ExternalId,
        CreatedBy = request.CreatedBy,
        UpdatedBy = request.UpdatedBy
    };

    public static TicketTaskResponse ToResponse(TicketTask entity) => new(
        entity.Id,
        entity.ExternalId,
        entity.CreatedAt,
        entity.UpdatedAt,
        entity.CreatedBy,
        entity.UpdatedBy,
        entity.Number,
        entity.ShortDescription,
        entity.Description,
        entity.RequesterId,
        entity.AffectedUserId,
        entity.AssignedAgentId,
        entity.AssignedGroupId,
        entity.ResolverId,
        entity.ResolutionGroupId,
        entity.CurrentStatusId,
        entity.PriorityId,
        entity.ImpactId,
        entity.UrgencyId,
        entity.SeverityId,
        entity.CategoryId,
        entity.SubcategoryId,
        entity.OpenedAt,
        entity.AcknowledgedAt,
        entity.AssignedAt,
        entity.FirstResponseAt,
        entity.PendingAt,
        entity.ResolvedAt,
        entity.ClosedAt,
        entity.DueAt,
        entity.SlaResponseDeadline,
        entity.SlaResolutionDeadline,
        entity.ReopenCount,
        entity.WasEscalated,
        entity.ParentTicketId);

    public static TicketAffectedCI ToEntity(TicketAffectedCIRequest request) => new()
    {
        TicketId = request.TicketId,
        ConfigurationItemId = request.ConfigurationItemId,
        IsPrimary = request.IsPrimary
    };

    public static TicketAffectedCIResponse ToResponse(TicketAffectedCI entity) => new(
        entity.TicketId,
        entity.ConfigurationItemId,
        entity.IsPrimary);

    public static TicketAffectedService ToEntity(TicketAffectedServiceRequest request) => new()
    {
        TicketId = request.TicketId,
        ServiceId = request.ServiceId,
        IsPrimary = request.IsPrimary
    };

    public static TicketAffectedServiceResponse ToResponse(TicketAffectedService entity) => new(
        entity.TicketId,
        entity.ServiceId,
        entity.IsPrimary);

    public static TicketRelation ToEntity(TicketRelationRequest request) => new()
    {
        SourceTicketId = request.SourceTicketId,
        TargetTicketId = request.TargetTicketId,
        RelationKind = request.RelationKind,
        CreatedAt = request.CreatedAt
    };

    public static TicketRelationResponse ToResponse(TicketRelation entity) => new(
        entity.Id,
        entity.SourceTicketId,
        entity.TargetTicketId,
        entity.RelationKind,
        entity.CreatedAt);
}
