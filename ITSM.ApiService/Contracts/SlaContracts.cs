using System.ComponentModel.DataAnnotations;
using GraphRag.ITSM.Entities;

namespace ITSM.ApiService.Contracts;

/// <summary>Payload for creating or updating a service level agreement.</summary>
public sealed record ServiceLevelAgreementRequest : EntityRequestBase
{
    [Required, MaxLength(200)]
    public required string Name { get; init; }

    public SlaKind Kind { get; init; }

    public TimeSpan TargetDuration { get; init; }

    public double? TargetAvailabilityPercent { get; init; }

    public Guid? AppliesToPriorityId { get; init; }

    public Guid? AppliesToServiceId { get; init; }

    public Guid? BusinessCalendarId { get; init; }

    public bool IsActive { get; init; } = true;
}

/// <summary>Represents a service level agreement returned by the API.</summary>
public sealed record ServiceLevelAgreementResponse(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    string Name,
    SlaKind Kind,
    TimeSpan TargetDuration,
    double? TargetAvailabilityPercent,
    Guid? AppliesToPriorityId,
    Guid? AppliesToServiceId,
    Guid? BusinessCalendarId,
    bool IsActive)
    : EntityResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy);

/// <summary>Payload for creating or updating an SLA measurement.</summary>
public sealed record SlaMeasurementRequest : EntityRequestBase
{
    public Guid TicketId { get; init; }

    public Guid ServiceLevelAgreementId { get; init; }

    public DateTimeOffset StartTime { get; init; }

    public DateTimeOffset? StopTime { get; init; }

    public TimeSpan PauseDuration { get; init; }

    public TimeSpan TargetDuration { get; init; }

    public TimeSpan? ElapsedDuration { get; init; }

    public SlaBreachStatus BreachStatus { get; init; }
}

/// <summary>Represents an SLA measurement returned by the API.</summary>
public sealed record SlaMeasurementResponse(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    Guid TicketId,
    Guid ServiceLevelAgreementId,
    DateTimeOffset StartTime,
    DateTimeOffset? StopTime,
    TimeSpan PauseDuration,
    TimeSpan TargetDuration,
    TimeSpan? ElapsedDuration,
    SlaBreachStatus BreachStatus)
    : EntityResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy);

/// <summary>Payload for creating or updating a business calendar.</summary>
public sealed record BusinessCalendarRequest : EntityRequestBase
{
    [Required, MaxLength(200)]
    public required string Name { get; init; }

    [Required, MaxLength(200)]
    public required string TimeZoneId { get; init; }

    public int WorkingDaysMask { get; init; }

    public TimeOnly? WorkingHoursStart { get; init; }

    public TimeOnly? WorkingHoursEnd { get; init; }
}

/// <summary>Represents a business calendar returned by the API.</summary>
public sealed record BusinessCalendarResponse(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    string Name,
    string TimeZoneId,
    int WorkingDaysMask,
    TimeOnly? WorkingHoursStart,
    TimeOnly? WorkingHoursEnd)
    : EntityResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy);

/// <summary>Payload for creating or updating a calendar holiday.</summary>
public sealed record CalendarHolidayRequest
{
    public Guid BusinessCalendarId { get; init; }

    public DateOnly Date { get; init; }

    public string? Name { get; init; }
}

/// <summary>Represents a calendar holiday returned by the API.</summary>
public sealed record CalendarHolidayResponse(Guid Id, Guid BusinessCalendarId, DateOnly Date, string? Name);

internal static partial class ItsmContractMapper
{
    public static ServiceLevelAgreement ToEntity(ServiceLevelAgreementRequest request) => new()
    {
        Name = request.Name,
        Kind = request.Kind,
        TargetDuration = request.TargetDuration,
        TargetAvailabilityPercent = request.TargetAvailabilityPercent,
        AppliesToPriorityId = request.AppliesToPriorityId,
        AppliesToServiceId = request.AppliesToServiceId,
        BusinessCalendarId = request.BusinessCalendarId,
        IsActive = request.IsActive,
        ExternalId = request.ExternalId,
        CreatedBy = request.CreatedBy,
        UpdatedBy = request.UpdatedBy
    };

    public static ServiceLevelAgreementResponse ToResponse(ServiceLevelAgreement entity) => new(
        entity.Id,
        entity.ExternalId,
        entity.CreatedAt,
        entity.UpdatedAt,
        entity.CreatedBy,
        entity.UpdatedBy,
        entity.Name,
        entity.Kind,
        entity.TargetDuration,
        entity.TargetAvailabilityPercent,
        entity.AppliesToPriorityId,
        entity.AppliesToServiceId,
        entity.BusinessCalendarId,
        entity.IsActive);

    public static SlaMeasurement ToEntity(SlaMeasurementRequest request) => new()
    {
        TicketId = request.TicketId,
        ServiceLevelAgreementId = request.ServiceLevelAgreementId,
        StartTime = request.StartTime,
        StopTime = request.StopTime,
        PauseDuration = request.PauseDuration,
        TargetDuration = request.TargetDuration,
        ElapsedDuration = request.ElapsedDuration,
        BreachStatus = request.BreachStatus,
        ExternalId = request.ExternalId,
        CreatedBy = request.CreatedBy,
        UpdatedBy = request.UpdatedBy
    };

    public static SlaMeasurementResponse ToResponse(SlaMeasurement entity) => new(
        entity.Id,
        entity.ExternalId,
        entity.CreatedAt,
        entity.UpdatedAt,
        entity.CreatedBy,
        entity.UpdatedBy,
        entity.TicketId,
        entity.ServiceLevelAgreementId,
        entity.StartTime,
        entity.StopTime,
        entity.PauseDuration,
        entity.TargetDuration,
        entity.ElapsedDuration,
        entity.BreachStatus);

    public static BusinessCalendar ToEntity(BusinessCalendarRequest request) => new()
    {
        Name = request.Name,
        TimeZoneId = request.TimeZoneId,
        WorkingDaysMask = request.WorkingDaysMask,
        WorkingHoursStart = request.WorkingHoursStart,
        WorkingHoursEnd = request.WorkingHoursEnd,
        ExternalId = request.ExternalId,
        CreatedBy = request.CreatedBy,
        UpdatedBy = request.UpdatedBy
    };

    public static BusinessCalendarResponse ToResponse(BusinessCalendar entity) => new(
        entity.Id,
        entity.ExternalId,
        entity.CreatedAt,
        entity.UpdatedAt,
        entity.CreatedBy,
        entity.UpdatedBy,
        entity.Name,
        entity.TimeZoneId,
        entity.WorkingDaysMask,
        entity.WorkingHoursStart,
        entity.WorkingHoursEnd);

    public static CalendarHoliday ToEntity(CalendarHolidayRequest request) => new()
    {
        BusinessCalendarId = request.BusinessCalendarId,
        Date = request.Date,
        Name = request.Name
    };

    public static CalendarHolidayResponse ToResponse(CalendarHoliday entity) => new(
        entity.Id,
        entity.BusinessCalendarId,
        entity.Date,
        entity.Name);
}
