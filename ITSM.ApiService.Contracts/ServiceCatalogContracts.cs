using System.ComponentModel.DataAnnotations;
using GraphRag.ITSM.Entities;

namespace ITSM.ApiService.Contracts;

public abstract record ServiceEntityRequestBase : EntityRequestBase
{
    [Required, MaxLength(200)]
    public required string Name { get; init; }

    public string? Description { get; init; }

    public bool IsActive { get; init; } = true;

    public Guid? OwnerId { get; init; }
}

public abstract record ServiceEntityResponseBase(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    string Name,
    string? Description,
    bool IsActive,
    Guid? OwnerId)
    : EntityResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy);

/// <summary>Payload for creating or updating a business service.</summary>
public sealed record BusinessServiceRequest : ServiceEntityRequestBase
{
    public string? BusinessCriticality { get; init; }
}

/// <summary>Represents a business service returned by the API.</summary>
public sealed record BusinessServiceResponse(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    string Name,
    string? Description,
    bool IsActive,
    Guid? OwnerId,
    string? BusinessCriticality)
    : ServiceEntityResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, Name, Description, IsActive, OwnerId);

/// <summary>Payload for creating or updating a technical service.</summary>
public sealed record TechnicalServiceRequest : ServiceEntityRequestBase
{
    public string? EnvironmentName { get; init; }
}

/// <summary>Represents a technical service returned by the API.</summary>
public sealed record TechnicalServiceResponse(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    string Name,
    string? Description,
    bool IsActive,
    Guid? OwnerId,
    string? EnvironmentName)
    : ServiceEntityResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, Name, Description, IsActive, OwnerId);

/// <summary>Payload for creating or updating a service offering.</summary>
public sealed record ServiceOfferingRequest : ServiceEntityRequestBase
{
    public string? SupportTier { get; init; }

    public Guid? BusinessServiceId { get; init; }
}

/// <summary>Represents a service offering returned by the API.</summary>
public sealed record ServiceOfferingResponse(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    string Name,
    string? Description,
    bool IsActive,
    Guid? OwnerId,
    string? SupportTier,
    Guid? BusinessServiceId)
    : ServiceEntityResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, Name, Description, IsActive, OwnerId);

/// <summary>Payload for creating or updating a service catalog item.</summary>
public sealed record ServiceCatalogItemRequest : ServiceEntityRequestBase
{
    public string? CatalogCategory { get; init; }

    public TimeSpan? StandardFulfillmentTime { get; init; }
}

/// <summary>Represents a service catalog item returned by the API.</summary>
public sealed record ServiceCatalogItemResponse(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    string Name,
    string? Description,
    bool IsActive,
    Guid? OwnerId,
    string? CatalogCategory,
    TimeSpan? StandardFulfillmentTime)
    : ServiceEntityResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, Name, Description, IsActive, OwnerId);

/// <summary>Payload for creating or updating a service support group link.</summary>
public sealed record ServiceSupportGroupRequest
{
    public Guid ServiceId { get; init; }

    public Guid AssignmentGroupId { get; init; }

    public bool IsPrimary { get; init; }
}

/// <summary>Represents a service support group link returned by the API.</summary>
public sealed record ServiceSupportGroupResponse(Guid ServiceId, Guid AssignmentGroupId, bool IsPrimary);

/// <summary>Payload for creating or updating a technical-service enablement link.</summary>
public sealed record TechnicalServiceEnablementRequest
{
    public Guid TechnicalServiceId { get; init; }

    public Guid BusinessServiceId { get; init; }
}

/// <summary>Represents a technical-service enablement link returned by the API.</summary>
public sealed record TechnicalServiceEnablementResponse(Guid TechnicalServiceId, Guid BusinessServiceId);

/// <summary>Payload for creating or updating a business-service consumer link.</summary>
public sealed record BusinessServiceConsumerRequest
{
    public Guid BusinessServiceId { get; init; }

    public Guid OrganizationId { get; init; }
}

/// <summary>Represents a business-service consumer link returned by the API.</summary>
public sealed record BusinessServiceConsumerResponse(Guid BusinessServiceId, Guid OrganizationId);

public static partial class ItsmContractMapper
{
    public static BusinessService ToEntity(BusinessServiceRequest request) => new()
    {
        Name = request.Name,
        Description = request.Description,
        IsActive = request.IsActive,
        OwnerId = request.OwnerId,
        BusinessCriticality = request.BusinessCriticality,
        ExternalId = request.ExternalId,
        CreatedBy = request.CreatedBy,
        UpdatedBy = request.UpdatedBy
    };

    public static BusinessServiceResponse ToResponse(BusinessService entity) => new(
        entity.Id,
        entity.ExternalId,
        entity.CreatedAt,
        entity.UpdatedAt,
        entity.CreatedBy,
        entity.UpdatedBy,
        entity.Name,
        entity.Description,
        entity.IsActive,
        entity.OwnerId,
        entity.BusinessCriticality);

    public static TechnicalService ToEntity(TechnicalServiceRequest request) => new()
    {
        Name = request.Name,
        Description = request.Description,
        IsActive = request.IsActive,
        OwnerId = request.OwnerId,
        EnvironmentName = request.EnvironmentName,
        ExternalId = request.ExternalId,
        CreatedBy = request.CreatedBy,
        UpdatedBy = request.UpdatedBy
    };

    public static TechnicalServiceResponse ToResponse(TechnicalService entity) => new(
        entity.Id,
        entity.ExternalId,
        entity.CreatedAt,
        entity.UpdatedAt,
        entity.CreatedBy,
        entity.UpdatedBy,
        entity.Name,
        entity.Description,
        entity.IsActive,
        entity.OwnerId,
        entity.EnvironmentName);

    public static ServiceOffering ToEntity(ServiceOfferingRequest request) => new()
    {
        Name = request.Name,
        Description = request.Description,
        IsActive = request.IsActive,
        OwnerId = request.OwnerId,
        SupportTier = request.SupportTier,
        BusinessServiceId = request.BusinessServiceId,
        ExternalId = request.ExternalId,
        CreatedBy = request.CreatedBy,
        UpdatedBy = request.UpdatedBy
    };

    public static ServiceOfferingResponse ToResponse(ServiceOffering entity) => new(
        entity.Id,
        entity.ExternalId,
        entity.CreatedAt,
        entity.UpdatedAt,
        entity.CreatedBy,
        entity.UpdatedBy,
        entity.Name,
        entity.Description,
        entity.IsActive,
        entity.OwnerId,
        entity.SupportTier,
        entity.BusinessServiceId);

    public static ServiceCatalogItem ToEntity(ServiceCatalogItemRequest request) => new()
    {
        Name = request.Name,
        Description = request.Description,
        IsActive = request.IsActive,
        OwnerId = request.OwnerId,
        CatalogCategory = request.CatalogCategory,
        StandardFulfillmentTime = request.StandardFulfillmentTime,
        ExternalId = request.ExternalId,
        CreatedBy = request.CreatedBy,
        UpdatedBy = request.UpdatedBy
    };

    public static ServiceCatalogItemResponse ToResponse(ServiceCatalogItem entity) => new(
        entity.Id,
        entity.ExternalId,
        entity.CreatedAt,
        entity.UpdatedAt,
        entity.CreatedBy,
        entity.UpdatedBy,
        entity.Name,
        entity.Description,
        entity.IsActive,
        entity.OwnerId,
        entity.CatalogCategory,
        entity.StandardFulfillmentTime);

    public static ServiceSupportGroup ToEntity(ServiceSupportGroupRequest request) => new()
    {
        ServiceId = request.ServiceId,
        AssignmentGroupId = request.AssignmentGroupId,
        IsPrimary = request.IsPrimary
    };

    public static ServiceSupportGroupResponse ToResponse(ServiceSupportGroup entity) => new(
        entity.ServiceId,
        entity.AssignmentGroupId,
        entity.IsPrimary);

    public static TechnicalServiceEnablement ToEntity(TechnicalServiceEnablementRequest request) => new()
    {
        TechnicalServiceId = request.TechnicalServiceId,
        BusinessServiceId = request.BusinessServiceId
    };

    public static TechnicalServiceEnablementResponse ToResponse(TechnicalServiceEnablement entity) => new(
        entity.TechnicalServiceId,
        entity.BusinessServiceId);

    public static BusinessServiceConsumer ToEntity(BusinessServiceConsumerRequest request) => new()
    {
        BusinessServiceId = request.BusinessServiceId,
        OrganizationId = request.OrganizationId
    };

    public static BusinessServiceConsumerResponse ToResponse(BusinessServiceConsumer entity) => new(
        entity.BusinessServiceId,
        entity.OrganizationId);
}
