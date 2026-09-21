using GraphRag.ITSM.Entities;

namespace ITSM.ApiService.Contracts;

public abstract record OperationalEventRequestBase : EntityRequestBase
{
    public Guid TicketId { get; init; }

    public DateTimeOffset EventTimestamp { get; init; }

    public Guid? PerformedById { get; init; }

    public string? Comment { get; init; }
}

public abstract record OperationalEventResponseBase(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    Guid TicketId,
    DateTimeOffset EventTimestamp,
    Guid? PerformedById,
    string? Comment)
    : EntityResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy);

/// <summary>Payload for creating or updating a status transition event.</summary>
public sealed record StatusTransitionRequest : OperationalEventRequestBase
{
    public Guid FromStatusId { get; init; }

    public Guid ToStatusId { get; init; }

    public TimeSpan? TimeInFromStatus { get; init; }
}

/// <summary>Represents a status transition event returned by the API.</summary>
public sealed record StatusTransitionResponse(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    Guid TicketId,
    DateTimeOffset EventTimestamp,
    Guid? PerformedById,
    string? Comment,
    Guid FromStatusId,
    Guid ToStatusId,
    TimeSpan? TimeInFromStatus)
    : OperationalEventResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, TicketId, EventTimestamp, PerformedById, Comment);

/// <summary>Payload for creating or updating an assignment event.</summary>
public sealed record AssignmentEventRequest : OperationalEventRequestBase
{
    public Guid? FromGroupId { get; init; }

    public Guid? ToGroupId { get; init; }

    public Guid? FromAgentId { get; init; }

    public Guid? ToAgentId { get; init; }
}

/// <summary>Represents an assignment event returned by the API.</summary>
public sealed record AssignmentEventResponse(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    Guid TicketId,
    DateTimeOffset EventTimestamp,
    Guid? PerformedById,
    string? Comment,
    Guid? FromGroupId,
    Guid? ToGroupId,
    Guid? FromAgentId,
    Guid? ToAgentId)
    : OperationalEventResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, TicketId, EventTimestamp, PerformedById, Comment);

/// <summary>Payload for creating or updating an escalation event.</summary>
public sealed record EscalationEventRequest : OperationalEventRequestBase
{
    public string? Reason { get; init; }

    public string? NewTier { get; init; }
}

/// <summary>Represents an escalation event returned by the API.</summary>
public sealed record EscalationEventResponse(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    Guid TicketId,
    DateTimeOffset EventTimestamp,
    Guid? PerformedById,
    string? Comment,
    string? Reason,
    string? NewTier)
    : OperationalEventResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, TicketId, EventTimestamp, PerformedById, Comment);

/// <summary>Payload for creating or updating a reassignment event.</summary>
public sealed record ReassignmentEventRequest : OperationalEventRequestBase
{
    public string? Reason { get; init; }
}

/// <summary>Represents a reassignment event returned by the API.</summary>
public sealed record ReassignmentEventResponse(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    Guid TicketId,
    DateTimeOffset EventTimestamp,
    Guid? PerformedById,
    string? Comment,
    string? Reason)
    : OperationalEventResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, TicketId, EventTimestamp, PerformedById, Comment);

/// <summary>Payload for creating or updating a resolution event.</summary>
public sealed record ResolutionEventRequest : OperationalEventRequestBase
{
    public string? ResolutionCode { get; init; }

    public string? ResolutionNotes { get; init; }
}

/// <summary>Represents a resolution event returned by the API.</summary>
public sealed record ResolutionEventResponse(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    Guid TicketId,
    DateTimeOffset EventTimestamp,
    Guid? PerformedById,
    string? Comment,
    string? ResolutionCode,
    string? ResolutionNotes)
    : OperationalEventResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, TicketId, EventTimestamp, PerformedById, Comment);

/// <summary>Payload for creating or updating a reopen event.</summary>
public sealed record ReopenEventRequest : OperationalEventRequestBase
{
    public string? Reason { get; init; }
}

/// <summary>Represents a reopen event returned by the API.</summary>
public sealed record ReopenEventResponse(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    Guid TicketId,
    DateTimeOffset EventTimestamp,
    Guid? PerformedById,
    string? Comment,
    string? Reason)
    : OperationalEventResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, TicketId, EventTimestamp, PerformedById, Comment);

/// <summary>Payload for creating or updating a closure event.</summary>
public sealed record ClosureEventRequest : OperationalEventRequestBase
{
    public string? ClosureCode { get; init; }
}

/// <summary>Represents a closure event returned by the API.</summary>
public sealed record ClosureEventResponse(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    Guid TicketId,
    DateTimeOffset EventTimestamp,
    Guid? PerformedById,
    string? Comment,
    string? ClosureCode)
    : OperationalEventResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, TicketId, EventTimestamp, PerformedById, Comment);

public static partial class ItsmContractMapper
{
    public static StatusTransition ToEntity(StatusTransitionRequest request) => new()
    {
        TicketId = request.TicketId,
        EventTimestamp = request.EventTimestamp,
        PerformedById = request.PerformedById,
        Comment = request.Comment,
        FromStatusId = request.FromStatusId,
        ToStatusId = request.ToStatusId,
        TimeInFromStatus = request.TimeInFromStatus,
        ExternalId = request.ExternalId,
        CreatedBy = request.CreatedBy,
        UpdatedBy = request.UpdatedBy
    };

    public static StatusTransitionResponse ToResponse(StatusTransition entity) => new(
        entity.Id,
        entity.ExternalId,
        entity.CreatedAt,
        entity.UpdatedAt,
        entity.CreatedBy,
        entity.UpdatedBy,
        entity.TicketId,
        entity.EventTimestamp,
        entity.PerformedById,
        entity.Comment,
        entity.FromStatusId,
        entity.ToStatusId,
        entity.TimeInFromStatus);

    public static AssignmentEvent ToEntity(AssignmentEventRequest request) => new()
    {
        TicketId = request.TicketId,
        EventTimestamp = request.EventTimestamp,
        PerformedById = request.PerformedById,
        Comment = request.Comment,
        FromGroupId = request.FromGroupId,
        ToGroupId = request.ToGroupId,
        FromAgentId = request.FromAgentId,
        ToAgentId = request.ToAgentId,
        ExternalId = request.ExternalId,
        CreatedBy = request.CreatedBy,
        UpdatedBy = request.UpdatedBy
    };

    public static AssignmentEventResponse ToResponse(AssignmentEvent entity) => new(
        entity.Id,
        entity.ExternalId,
        entity.CreatedAt,
        entity.UpdatedAt,
        entity.CreatedBy,
        entity.UpdatedBy,
        entity.TicketId,
        entity.EventTimestamp,
        entity.PerformedById,
        entity.Comment,
        entity.FromGroupId,
        entity.ToGroupId,
        entity.FromAgentId,
        entity.ToAgentId);

    public static EscalationEvent ToEntity(EscalationEventRequest request) => new()
    {
        TicketId = request.TicketId,
        EventTimestamp = request.EventTimestamp,
        PerformedById = request.PerformedById,
        Comment = request.Comment,
        Reason = request.Reason,
        NewTier = request.NewTier,
        ExternalId = request.ExternalId,
        CreatedBy = request.CreatedBy,
        UpdatedBy = request.UpdatedBy
    };

    public static EscalationEventResponse ToResponse(EscalationEvent entity) => new(
        entity.Id,
        entity.ExternalId,
        entity.CreatedAt,
        entity.UpdatedAt,
        entity.CreatedBy,
        entity.UpdatedBy,
        entity.TicketId,
        entity.EventTimestamp,
        entity.PerformedById,
        entity.Comment,
        entity.Reason,
        entity.NewTier);

    public static ReassignmentEvent ToEntity(ReassignmentEventRequest request) => new()
    {
        TicketId = request.TicketId,
        EventTimestamp = request.EventTimestamp,
        PerformedById = request.PerformedById,
        Comment = request.Comment,
        Reason = request.Reason,
        ExternalId = request.ExternalId,
        CreatedBy = request.CreatedBy,
        UpdatedBy = request.UpdatedBy
    };

    public static ReassignmentEventResponse ToResponse(ReassignmentEvent entity) => new(
        entity.Id,
        entity.ExternalId,
        entity.CreatedAt,
        entity.UpdatedAt,
        entity.CreatedBy,
        entity.UpdatedBy,
        entity.TicketId,
        entity.EventTimestamp,
        entity.PerformedById,
        entity.Comment,
        entity.Reason);

    public static ResolutionEvent ToEntity(ResolutionEventRequest request) => new()
    {
        TicketId = request.TicketId,
        EventTimestamp = request.EventTimestamp,
        PerformedById = request.PerformedById,
        Comment = request.Comment,
        ResolutionCode = request.ResolutionCode,
        ResolutionNotes = request.ResolutionNotes,
        ExternalId = request.ExternalId,
        CreatedBy = request.CreatedBy,
        UpdatedBy = request.UpdatedBy
    };

    public static ResolutionEventResponse ToResponse(ResolutionEvent entity) => new(
        entity.Id,
        entity.ExternalId,
        entity.CreatedAt,
        entity.UpdatedAt,
        entity.CreatedBy,
        entity.UpdatedBy,
        entity.TicketId,
        entity.EventTimestamp,
        entity.PerformedById,
        entity.Comment,
        entity.ResolutionCode,
        entity.ResolutionNotes);

    public static ReopenEvent ToEntity(ReopenEventRequest request) => new()
    {
        TicketId = request.TicketId,
        EventTimestamp = request.EventTimestamp,
        PerformedById = request.PerformedById,
        Comment = request.Comment,
        Reason = request.Reason,
        ExternalId = request.ExternalId,
        CreatedBy = request.CreatedBy,
        UpdatedBy = request.UpdatedBy
    };

    public static ReopenEventResponse ToResponse(ReopenEvent entity) => new(
        entity.Id,
        entity.ExternalId,
        entity.CreatedAt,
        entity.UpdatedAt,
        entity.CreatedBy,
        entity.UpdatedBy,
        entity.TicketId,
        entity.EventTimestamp,
        entity.PerformedById,
        entity.Comment,
        entity.Reason);

    public static ClosureEvent ToEntity(ClosureEventRequest request) => new()
    {
        TicketId = request.TicketId,
        EventTimestamp = request.EventTimestamp,
        PerformedById = request.PerformedById,
        Comment = request.Comment,
        ClosureCode = request.ClosureCode,
        ExternalId = request.ExternalId,
        CreatedBy = request.CreatedBy,
        UpdatedBy = request.UpdatedBy
    };

    public static ClosureEventResponse ToResponse(ClosureEvent entity) => new(
        entity.Id,
        entity.ExternalId,
        entity.CreatedAt,
        entity.UpdatedAt,
        entity.CreatedBy,
        entity.UpdatedBy,
        entity.TicketId,
        entity.EventTimestamp,
        entity.PerformedById,
        entity.Comment,
        entity.ClosureCode);
}
