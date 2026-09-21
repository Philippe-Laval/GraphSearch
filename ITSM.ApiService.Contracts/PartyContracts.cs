using System.ComponentModel.DataAnnotations;
using GraphRag.ITSM.Entities;

namespace ITSM.ApiService.Contracts;

public abstract record PersonRequestBase : EntityRequestBase
{
    [Required, MaxLength(200)]
    public required string FullName { get; init; }

    [EmailAddress]
    public string? Email { get; init; }

    public string? Department { get; init; }

    public string? Location { get; init; }

    public bool IsActive { get; init; } = true;

    public Guid? OrganizationId { get; init; }
}

public abstract record PersonResponseBase(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    string FullName,
    string? Email,
    string? Department,
    string? Location,
    bool IsActive,
    Guid? OrganizationId)
    : EntityResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy);

/// <summary>Payload for creating or updating a person.</summary>
public sealed record PersonRequest : PersonRequestBase;

/// <summary>Represents a person returned by the API.</summary>
public sealed record PersonResponse(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    string FullName,
    string? Email,
    string? Department,
    string? Location,
    bool IsActive,
    Guid? OrganizationId)
    : PersonResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, FullName, Email, Department, Location, IsActive, OrganizationId);

/// <summary>Payload for creating or updating an agent.</summary>
public sealed record AgentRequest : PersonRequestBase
{
    public string? SupportTier { get; init; }
}

/// <summary>Represents an agent returned by the API.</summary>
public sealed record AgentResponse(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    string FullName,
    string? Email,
    string? Department,
    string? Location,
    bool IsActive,
    Guid? OrganizationId,
    string? SupportTier)
    : PersonResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, FullName, Email, Department, Location, IsActive, OrganizationId);

/// <summary>Payload for creating or updating an organization.</summary>
public sealed record OrganizationRequest : EntityRequestBase
{
    [Required, MaxLength(200)]
    public required string Name { get; init; }

    public string? BusinessUnit { get; init; }

    public Guid? ParentOrganizationId { get; init; }
}

/// <summary>Represents an organization returned by the API.</summary>
public sealed record OrganizationResponse(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    string Name,
    string? BusinessUnit,
    Guid? ParentOrganizationId)
    : EntityResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy);

/// <summary>Payload for creating or updating an assignment group.</summary>
public sealed record AssignmentGroupRequest : EntityRequestBase
{
    [Required, MaxLength(200)]
    public required string Name { get; init; }

    public string? Department { get; init; }

    public string? Description { get; init; }

    public string? SupportTier { get; init; }

    public bool IsActive { get; init; } = true;
}

/// <summary>Represents an assignment group returned by the API.</summary>
public sealed record AssignmentGroupResponse(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    string Name,
    string? Department,
    string? Description,
    string? SupportTier,
    bool IsActive)
    : EntityResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy);

/// <summary>Payload for creating or updating a vendor.</summary>
public sealed record VendorRequest : EntityRequestBase
{
    [Required, MaxLength(200)]
    public required string Name { get; init; }

    public string? ContractReference { get; init; }
}

/// <summary>Represents a vendor returned by the API.</summary>
public sealed record VendorResponse(
    Guid Id,
    string? ExternalId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    string Name,
    string? ContractReference)
    : EntityResponseBase(Id, ExternalId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy);

/// <summary>Payload for creating or updating an agent-group membership.</summary>
public sealed record AgentGroupMembershipRequest
{
    public Guid AgentId { get; init; }

    public Guid AssignmentGroupId { get; init; }

    public DateTimeOffset AssignedAt { get; init; }

    public DateTimeOffset? RemovedAt { get; init; }

    public string? RoleInGroup { get; init; }
}

/// <summary>Represents an agent-group membership returned by the API.</summary>
public sealed record AgentGroupMembershipResponse(
    Guid AgentId,
    Guid AssignmentGroupId,
    DateTimeOffset AssignedAt,
    DateTimeOffset? RemovedAt,
    string? RoleInGroup);

public static partial class ItsmContractMapper
{
    public static Person ToEntity(PersonRequest request) => new()
    {
        FullName = request.FullName,
        Email = request.Email,
        Department = request.Department,
        Location = request.Location,
        IsActive = request.IsActive,
        OrganizationId = request.OrganizationId,
        ExternalId = request.ExternalId,
        CreatedBy = request.CreatedBy,
        UpdatedBy = request.UpdatedBy
    };

    public static PersonResponse ToResponse(Person entity) => new(
        entity.Id,
        entity.ExternalId,
        entity.CreatedAt,
        entity.UpdatedAt,
        entity.CreatedBy,
        entity.UpdatedBy,
        entity.FullName,
        entity.Email,
        entity.Department,
        entity.Location,
        entity.IsActive,
        entity.OrganizationId);

    public static Agent ToEntity(AgentRequest request) => new()
    {
        FullName = request.FullName,
        Email = request.Email,
        Department = request.Department,
        Location = request.Location,
        IsActive = request.IsActive,
        OrganizationId = request.OrganizationId,
        SupportTier = request.SupportTier,
        ExternalId = request.ExternalId,
        CreatedBy = request.CreatedBy,
        UpdatedBy = request.UpdatedBy
    };

    public static AgentResponse ToResponse(Agent entity) => new(
        entity.Id,
        entity.ExternalId,
        entity.CreatedAt,
        entity.UpdatedAt,
        entity.CreatedBy,
        entity.UpdatedBy,
        entity.FullName,
        entity.Email,
        entity.Department,
        entity.Location,
        entity.IsActive,
        entity.OrganizationId,
        entity.SupportTier);

    public static Organization ToEntity(OrganizationRequest request) => new()
    {
        Name = request.Name,
        BusinessUnit = request.BusinessUnit,
        ParentOrganizationId = request.ParentOrganizationId,
        ExternalId = request.ExternalId,
        CreatedBy = request.CreatedBy,
        UpdatedBy = request.UpdatedBy
    };

    public static OrganizationResponse ToResponse(Organization entity) => new(
        entity.Id,
        entity.ExternalId,
        entity.CreatedAt,
        entity.UpdatedAt,
        entity.CreatedBy,
        entity.UpdatedBy,
        entity.Name,
        entity.BusinessUnit,
        entity.ParentOrganizationId);

    public static AssignmentGroup ToEntity(AssignmentGroupRequest request) => new()
    {
        Name = request.Name,
        Department = request.Department,
        Description = request.Description,
        SupportTier = request.SupportTier,
        IsActive = request.IsActive,
        ExternalId = request.ExternalId,
        CreatedBy = request.CreatedBy,
        UpdatedBy = request.UpdatedBy
    };

    public static AssignmentGroupResponse ToResponse(AssignmentGroup entity) => new(
        entity.Id,
        entity.ExternalId,
        entity.CreatedAt,
        entity.UpdatedAt,
        entity.CreatedBy,
        entity.UpdatedBy,
        entity.Name,
        entity.Department,
        entity.Description,
        entity.SupportTier,
        entity.IsActive);

    public static Vendor ToEntity(VendorRequest request) => new()
    {
        Name = request.Name,
        ContractReference = request.ContractReference,
        ExternalId = request.ExternalId,
        CreatedBy = request.CreatedBy,
        UpdatedBy = request.UpdatedBy
    };

    public static VendorResponse ToResponse(Vendor entity) => new(
        entity.Id,
        entity.ExternalId,
        entity.CreatedAt,
        entity.UpdatedAt,
        entity.CreatedBy,
        entity.UpdatedBy,
        entity.Name,
        entity.ContractReference);

    public static AgentGroupMembership ToEntity(AgentGroupMembershipRequest request) => new()
    {
        AgentId = request.AgentId,
        AssignmentGroupId = request.AssignmentGroupId,
        AssignedAt = request.AssignedAt,
        RemovedAt = request.RemovedAt,
        RoleInGroup = request.RoleInGroup
    };

    public static AgentGroupMembershipResponse ToResponse(AgentGroupMembership entity) => new(
        entity.AgentId,
        entity.AssignmentGroupId,
        entity.AssignedAt,
        entity.RemovedAt,
        entity.RoleInGroup);
}
