using System.ComponentModel.DataAnnotations;
using GraphRag.ITSM.Entities;

namespace ITSM.ApiService.Contracts;

public abstract record ConfigurationItemRequestBase : EntityRequestBase
{
    [Required, MaxLength(200)]
    public required string Name { get; init; }

    public string? AssetTag { get; init; }

    public CIOperationalStatus OperationalStatus { get; init; } = CIOperationalStatus.Deployed;

    public string? Environment { get; init; }

    public Guid? OwnerId { get; init; }
}

public abstract record ConfigurationItemResponseBase(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    string Name,
    string? AssetTag,
    CIOperationalStatus OperationalStatus,
    string? Environment,
    Guid? OwnerId)
    : EntityResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy);

/// <summary>Payload for creating or updating an application configuration item.</summary>
public sealed record ApplicationRequest : ConfigurationItemRequestBase
{
    public string? Vendor { get; init; }

    public string? VersionNumber { get; init; }
}

/// <summary>Represents an application configuration item returned by the API.</summary>
public sealed record ApplicationResponse(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    string Name,
    string? AssetTag,
    CIOperationalStatus OperationalStatus,
    string? Environment,
    Guid? OwnerId,
    string? Vendor,
    string? VersionNumber)
    : ConfigurationItemResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, Name, AssetTag, OperationalStatus, Environment, OwnerId);

/// <summary>Payload for creating or updating a server configuration item.</summary>
public sealed record ServerRequest : ConfigurationItemRequestBase
{
    public string? Hostname { get; init; }

    public string? OperatingSystem { get; init; }

    public string? IpAddress { get; init; }
}

/// <summary>Represents a server configuration item returned by the API.</summary>
public sealed record ServerResponse(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    string Name,
    string? AssetTag,
    CIOperationalStatus OperationalStatus,
    string? Environment,
    Guid? OwnerId,
    string? Hostname,
    string? OperatingSystem,
    string? IpAddress)
    : ConfigurationItemResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, Name, AssetTag, OperationalStatus, Environment, OwnerId);

/// <summary>Payload for creating or updating a database configuration item.</summary>
public sealed record DatabaseCIRequest : ConfigurationItemRequestBase
{
    public string? Engine { get; init; }

    public string? InstanceName { get; init; }
}

/// <summary>Represents a database configuration item returned by the API.</summary>
public sealed record DatabaseCIResponse(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    string Name,
    string? AssetTag,
    CIOperationalStatus OperationalStatus,
    string? Environment,
    Guid? OwnerId,
    string? Engine,
    string? InstanceName)
    : ConfigurationItemResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, Name, AssetTag, OperationalStatus, Environment, OwnerId);

/// <summary>Payload for creating or updating a network device.</summary>
public sealed record NetworkDeviceRequest : ConfigurationItemRequestBase
{
    public string? DeviceType { get; init; }

    public string? IpAddress { get; init; }
}

/// <summary>Represents a network device returned by the API.</summary>
public sealed record NetworkDeviceResponse(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    string Name,
    string? AssetTag,
    CIOperationalStatus OperationalStatus,
    string? Environment,
    Guid? OwnerId,
    string? DeviceType,
    string? IpAddress)
    : ConfigurationItemResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, Name, AssetTag, OperationalStatus, Environment, OwnerId);

/// <summary>Payload for creating or updating a cloud resource.</summary>
public sealed record CloudResourceRequest : ConfigurationItemRequestBase
{
    public string? Provider { get; init; }

    public string? Region { get; init; }

    public string? ResourceType { get; init; }
}

/// <summary>Represents a cloud resource returned by the API.</summary>
public sealed record CloudResourceResponse(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    string Name,
    string? AssetTag,
    CIOperationalStatus OperationalStatus,
    string? Environment,
    Guid? OwnerId,
    string? Provider,
    string? Region,
    string? ResourceType)
    : ConfigurationItemResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, Name, AssetTag, OperationalStatus, Environment, OwnerId);

/// <summary>Payload for creating or updating a configuration-item support link.</summary>
public sealed record ConfigurationItemSupportRequest
{
    public Guid ConfigurationItemId { get; init; }

    public Guid TechnicalServiceId { get; init; }
}

/// <summary>Represents a configuration-item support link returned by the API.</summary>
public sealed record ConfigurationItemSupportResponse(Guid ConfigurationItemId, Guid TechnicalServiceId);

/// <summary>Payload for creating or updating a configuration-item dependency.</summary>
public sealed record ConfigurationItemDependencyRequest
{
    public Guid SourceId { get; init; }

    public Guid TargetId { get; init; }

    public string? DependencyType { get; init; }
}

/// <summary>Represents a configuration-item dependency returned by the API.</summary>
public sealed record ConfigurationItemDependencyResponse(Guid SourceId, Guid TargetId, string? DependencyType);

internal static partial class ItsmContractMapper
{
    public static Application ToEntity(ApplicationRequest request) => new()
    {
        Name = request.Name,
        AssetTag = request.AssetTag,
        OperationalStatus = request.OperationalStatus,
        Environment = request.Environment,
        OwnerId = request.OwnerId,
        Vendor = request.Vendor,
        VersionNumber = request.VersionNumber,
        ExternalId = request.ExternalId,
        CreatedBy = request.CreatedBy,
        UpdatedBy = request.UpdatedBy
    };

    public static ApplicationResponse ToResponse(Application entity) => new(
        entity.Id,
        entity.ExternalId,
        entity.CreatedAt,
        entity.UpdatedAt,
        entity.CreatedBy,
        entity.UpdatedBy,
        entity.Name,
        entity.AssetTag,
        entity.OperationalStatus,
        entity.Environment,
        entity.OwnerId,
        entity.Vendor,
        entity.VersionNumber);

    public static Server ToEntity(ServerRequest request) => new()
    {
        Name = request.Name,
        AssetTag = request.AssetTag,
        OperationalStatus = request.OperationalStatus,
        Environment = request.Environment,
        OwnerId = request.OwnerId,
        Hostname = request.Hostname,
        OperatingSystem = request.OperatingSystem,
        IpAddress = request.IpAddress,
        ExternalId = request.ExternalId,
        CreatedBy = request.CreatedBy,
        UpdatedBy = request.UpdatedBy
    };

    public static ServerResponse ToResponse(Server entity) => new(
        entity.Id,
        entity.ExternalId,
        entity.CreatedAt,
        entity.UpdatedAt,
        entity.CreatedBy,
        entity.UpdatedBy,
        entity.Name,
        entity.AssetTag,
        entity.OperationalStatus,
        entity.Environment,
        entity.OwnerId,
        entity.Hostname,
        entity.OperatingSystem,
        entity.IpAddress);

    public static DatabaseCI ToEntity(DatabaseCIRequest request) => new()
    {
        Name = request.Name,
        AssetTag = request.AssetTag,
        OperationalStatus = request.OperationalStatus,
        Environment = request.Environment,
        OwnerId = request.OwnerId,
        Engine = request.Engine,
        InstanceName = request.InstanceName,
        ExternalId = request.ExternalId,
        CreatedBy = request.CreatedBy,
        UpdatedBy = request.UpdatedBy
    };

    public static DatabaseCIResponse ToResponse(DatabaseCI entity) => new(
        entity.Id,
        entity.ExternalId,
        entity.CreatedAt,
        entity.UpdatedAt,
        entity.CreatedBy,
        entity.UpdatedBy,
        entity.Name,
        entity.AssetTag,
        entity.OperationalStatus,
        entity.Environment,
        entity.OwnerId,
        entity.Engine,
        entity.InstanceName);

    public static NetworkDevice ToEntity(NetworkDeviceRequest request) => new()
    {
        Name = request.Name,
        AssetTag = request.AssetTag,
        OperationalStatus = request.OperationalStatus,
        Environment = request.Environment,
        OwnerId = request.OwnerId,
        DeviceType = request.DeviceType,
        IpAddress = request.IpAddress,
        ExternalId = request.ExternalId,
        CreatedBy = request.CreatedBy,
        UpdatedBy = request.UpdatedBy
    };

    public static NetworkDeviceResponse ToResponse(NetworkDevice entity) => new(
        entity.Id,
        entity.ExternalId,
        entity.CreatedAt,
        entity.UpdatedAt,
        entity.CreatedBy,
        entity.UpdatedBy,
        entity.Name,
        entity.AssetTag,
        entity.OperationalStatus,
        entity.Environment,
        entity.OwnerId,
        entity.DeviceType,
        entity.IpAddress);

    public static CloudResource ToEntity(CloudResourceRequest request) => new()
    {
        Name = request.Name,
        AssetTag = request.AssetTag,
        OperationalStatus = request.OperationalStatus,
        Environment = request.Environment,
        OwnerId = request.OwnerId,
        Provider = request.Provider,
        Region = request.Region,
        ResourceType = request.ResourceType,
        ExternalId = request.ExternalId,
        CreatedBy = request.CreatedBy,
        UpdatedBy = request.UpdatedBy
    };

    public static CloudResourceResponse ToResponse(CloudResource entity) => new(
        entity.Id,
        entity.ExternalId,
        entity.CreatedAt,
        entity.UpdatedAt,
        entity.CreatedBy,
        entity.UpdatedBy,
        entity.Name,
        entity.AssetTag,
        entity.OperationalStatus,
        entity.Environment,
        entity.OwnerId,
        entity.Provider,
        entity.Region,
        entity.ResourceType);

    public static ConfigurationItemSupport ToEntity(ConfigurationItemSupportRequest request) => new()
    {
        ConfigurationItemId = request.ConfigurationItemId,
        TechnicalServiceId = request.TechnicalServiceId
    };

    public static ConfigurationItemSupportResponse ToResponse(ConfigurationItemSupport entity) => new(
        entity.ConfigurationItemId,
        entity.TechnicalServiceId);

    public static ConfigurationItemDependency ToEntity(ConfigurationItemDependencyRequest request) => new()
    {
        SourceId = request.SourceId,
        TargetId = request.TargetId,
        DependencyType = request.DependencyType
    };

    public static ConfigurationItemDependencyResponse ToResponse(ConfigurationItemDependency entity) => new(
        entity.SourceId,
        entity.TargetId,
        entity.DependencyType);
}
