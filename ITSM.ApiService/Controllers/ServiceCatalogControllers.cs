using GraphRag.ITSM.Entities;
using GraphRag.ITSM.Services;
using ITSM.ApiService.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace ITSM.ApiService.Controllers;

[ApiController]
[Route("api/business-services")]
public sealed class BusinessServicesController(BusinessServiceService service)
    : EntityCrudControllerBase<BusinessService, BusinessServiceRequest, BusinessServiceResponse, BusinessServiceService>(
        service,
        ItsmContractMapper.ToEntity,
        ItsmContractMapper.ToResponse);

[ApiController]
[Route("api/technical-services")]
public sealed class TechnicalServicesController(TechnicalServiceService service)
    : EntityCrudControllerBase<TechnicalService, TechnicalServiceRequest, TechnicalServiceResponse, TechnicalServiceService>(
        service,
        ItsmContractMapper.ToEntity,
        ItsmContractMapper.ToResponse);

[ApiController]
[Route("api/service-offerings")]
public sealed class ServiceOfferingsController(ServiceOfferingService service)
    : EntityCrudControllerBase<ServiceOffering, ServiceOfferingRequest, ServiceOfferingResponse, ServiceOfferingService>(
        service,
        ItsmContractMapper.ToEntity,
        ItsmContractMapper.ToResponse);

[ApiController]
[Route("api/service-catalog-items")]
public sealed class ServiceCatalogItemsController(ServiceCatalogItemService service)
    : EntityCrudControllerBase<ServiceCatalogItem, ServiceCatalogItemRequest, ServiceCatalogItemResponse, ServiceCatalogItemService>(
        service,
        ItsmContractMapper.ToEntity,
        ItsmContractMapper.ToResponse);

[ApiController]
[Route("api/service-support-groups")]
public sealed class ServiceSupportGroupsController(ServiceSupportGroupService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ServiceSupportGroupResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var groups = await service.GetAllAsync(cancellationToken);
        return Ok(groups.Select(ItsmContractMapper.ToResponse).ToArray());
    }

    [HttpGet("{serviceId:guid}/{assignmentGroupId:guid}")]
    public async Task<ActionResult<ServiceSupportGroupResponse>> GetById(
        Guid serviceId,
        Guid assignmentGroupId,
        CancellationToken cancellationToken)
    {
        var group = await service.GetAsync(serviceId, assignmentGroupId, cancellationToken);
        return group is null ? NotFound() : Ok(ItsmContractMapper.ToResponse(group));
    }

    [HttpPost]
    public async Task<ActionResult<ServiceSupportGroupResponse>> Create(
        [FromBody] ServiceSupportGroupRequest request,
        CancellationToken cancellationToken)
    {
        var created = await service.CreateAsync(ItsmContractMapper.ToEntity(request), cancellationToken);
        return CreatedAtAction(
            nameof(GetById),
            new { serviceId = created.ServiceId, assignmentGroupId = created.AssignmentGroupId },
            ItsmContractMapper.ToResponse(created));
    }

    [HttpPut("{serviceId:guid}/{assignmentGroupId:guid}")]
    public async Task<ActionResult<ServiceSupportGroupResponse>> Update(
        Guid serviceId,
        Guid assignmentGroupId,
        [FromBody] ServiceSupportGroupRequest request,
        CancellationToken cancellationToken)
    {
        var group = ItsmContractMapper.ToEntity(request);
        group.ServiceId = serviceId;
        group.AssignmentGroupId = assignmentGroupId;

        var updated = await service.UpdateAsync(group, cancellationToken);
        return updated is null ? NotFound() : Ok(ItsmContractMapper.ToResponse(updated));
    }

    [HttpDelete("{serviceId:guid}/{assignmentGroupId:guid}")]
    public async Task<IActionResult> Delete(Guid serviceId, Guid assignmentGroupId, CancellationToken cancellationToken)
    {
        var deleted = await service.DeleteAsync(serviceId, assignmentGroupId, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}

[ApiController]
[Route("api/technical-service-enablements")]
public sealed class TechnicalServiceEnablementsController(TechnicalServiceEnablementService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TechnicalServiceEnablementResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var enablements = await service.GetAllAsync(cancellationToken);
        return Ok(enablements.Select(ItsmContractMapper.ToResponse).ToArray());
    }

    [HttpGet("{technicalServiceId:guid}/{businessServiceId:guid}")]
    public async Task<ActionResult<TechnicalServiceEnablementResponse>> GetById(
        Guid technicalServiceId,
        Guid businessServiceId,
        CancellationToken cancellationToken)
    {
        var enablement = await service.GetAsync(technicalServiceId, businessServiceId, cancellationToken);
        return enablement is null ? NotFound() : Ok(ItsmContractMapper.ToResponse(enablement));
    }

    [HttpPost]
    public async Task<ActionResult<TechnicalServiceEnablementResponse>> Create(
        [FromBody] TechnicalServiceEnablementRequest request,
        CancellationToken cancellationToken)
    {
        var created = await service.CreateAsync(ItsmContractMapper.ToEntity(request), cancellationToken);
        return CreatedAtAction(
            nameof(GetById),
            new { technicalServiceId = created.TechnicalServiceId, businessServiceId = created.BusinessServiceId },
            ItsmContractMapper.ToResponse(created));
    }

    [HttpDelete("{technicalServiceId:guid}/{businessServiceId:guid}")]
    public async Task<IActionResult> Delete(
        Guid technicalServiceId,
        Guid businessServiceId,
        CancellationToken cancellationToken)
    {
        var deleted = await service.DeleteAsync(technicalServiceId, businessServiceId, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}

[ApiController]
[Route("api/business-service-consumers")]
public sealed class BusinessServiceConsumersController(BusinessServiceConsumerService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BusinessServiceConsumerResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var consumers = await service.GetAllAsync(cancellationToken);
        return Ok(consumers.Select(ItsmContractMapper.ToResponse).ToArray());
    }

    [HttpGet("{businessServiceId:guid}/{organizationId:guid}")]
    public async Task<ActionResult<BusinessServiceConsumerResponse>> GetById(
        Guid businessServiceId,
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var consumer = await service.GetAsync(businessServiceId, organizationId, cancellationToken);
        return consumer is null ? NotFound() : Ok(ItsmContractMapper.ToResponse(consumer));
    }

    [HttpPost]
    public async Task<ActionResult<BusinessServiceConsumerResponse>> Create(
        [FromBody] BusinessServiceConsumerRequest request,
        CancellationToken cancellationToken)
    {
        var created = await service.CreateAsync(ItsmContractMapper.ToEntity(request), cancellationToken);
        return CreatedAtAction(
            nameof(GetById),
            new { businessServiceId = created.BusinessServiceId, organizationId = created.OrganizationId },
            ItsmContractMapper.ToResponse(created));
    }

    [HttpDelete("{businessServiceId:guid}/{organizationId:guid}")]
    public async Task<IActionResult> Delete(
        Guid businessServiceId,
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var deleted = await service.DeleteAsync(businessServiceId, organizationId, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
