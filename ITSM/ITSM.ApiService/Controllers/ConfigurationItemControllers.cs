using GraphRag.ITSM.Entities;
using GraphRag.ITSM.Services;
using ITSM.ApiService.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace ITSM.ApiService.Controllers;

[ApiController]
[Route("api/applications")]
public sealed class ApplicationsController(ApplicationService service)
    : EntityCrudControllerBase<Application, ApplicationRequest, ApplicationResponse, ApplicationService>(
        service,
        ItsmContractMapper.ToEntity,
        ItsmContractMapper.ToResponse);

[ApiController]
[Route("api/servers")]
public sealed class ServersController(ServerService service)
    : EntityCrudControllerBase<Server, ServerRequest, ServerResponse, ServerService>(
        service,
        ItsmContractMapper.ToEntity,
        ItsmContractMapper.ToResponse);

[ApiController]
[Route("api/database-cis")]
public sealed class DatabaseConfigurationItemsController(DatabaseCIService service)
    : EntityCrudControllerBase<DatabaseCI, DatabaseCIRequest, DatabaseCIResponse, DatabaseCIService>(
        service,
        ItsmContractMapper.ToEntity,
        ItsmContractMapper.ToResponse);

[ApiController]
[Route("api/network-devices")]
public sealed class NetworkDevicesController(NetworkDeviceService service)
    : EntityCrudControllerBase<NetworkDevice, NetworkDeviceRequest, NetworkDeviceResponse, NetworkDeviceService>(
        service,
        ItsmContractMapper.ToEntity,
        ItsmContractMapper.ToResponse);

[ApiController]
[Route("api/cloud-resources")]
public sealed class CloudResourcesController(CloudResourceService service)
    : EntityCrudControllerBase<CloudResource, CloudResourceRequest, CloudResourceResponse, CloudResourceService>(
        service,
        ItsmContractMapper.ToEntity,
        ItsmContractMapper.ToResponse);

[ApiController]
[Route("api/configuration-item-supports")]
public sealed class ConfigurationItemSupportsController(ConfigurationItemSupportService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ConfigurationItemSupportResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var supports = await service.GetAllAsync(cancellationToken);
        return Ok(supports.Select(ItsmContractMapper.ToResponse).ToArray());
    }

    [HttpGet("{configurationItemId:guid}/{technicalServiceId:guid}")]
    public async Task<ActionResult<ConfigurationItemSupportResponse>> GetById(
        Guid configurationItemId,
        Guid technicalServiceId,
        CancellationToken cancellationToken)
    {
        var support = await service.GetAsync(configurationItemId, technicalServiceId, cancellationToken);
        return support is null ? NotFound() : Ok(ItsmContractMapper.ToResponse(support));
    }

    [HttpPost]
    public async Task<ActionResult<ConfigurationItemSupportResponse>> Create(
        [FromBody] ConfigurationItemSupportRequest request,
        CancellationToken cancellationToken)
    {
        var created = await service.CreateAsync(ItsmContractMapper.ToEntity(request), cancellationToken);
        return CreatedAtAction(
            nameof(GetById),
            new { configurationItemId = created.ConfigurationItemId, technicalServiceId = created.TechnicalServiceId },
            ItsmContractMapper.ToResponse(created));
    }

    [HttpDelete("{configurationItemId:guid}/{technicalServiceId:guid}")]
    public async Task<IActionResult> Delete(
        Guid configurationItemId,
        Guid technicalServiceId,
        CancellationToken cancellationToken)
    {
        var deleted = await service.DeleteAsync(configurationItemId, technicalServiceId, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}

[ApiController]
[Route("api/configuration-item-dependencies")]
public sealed class ConfigurationItemDependenciesController(ConfigurationItemDependencyService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ConfigurationItemDependencyResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var dependencies = await service.GetAllAsync(cancellationToken);
        return Ok(dependencies.Select(ItsmContractMapper.ToResponse).ToArray());
    }

    [HttpGet("{sourceId:guid}/{targetId:guid}")]
    public async Task<ActionResult<ConfigurationItemDependencyResponse>> GetById(
        Guid sourceId,
        Guid targetId,
        CancellationToken cancellationToken)
    {
        var dependency = await service.GetAsync(sourceId, targetId, cancellationToken);
        return dependency is null ? NotFound() : Ok(ItsmContractMapper.ToResponse(dependency));
    }

    [HttpPost]
    public async Task<ActionResult<ConfigurationItemDependencyResponse>> Create(
        [FromBody] ConfigurationItemDependencyRequest request,
        CancellationToken cancellationToken)
    {
        var created = await service.CreateAsync(ItsmContractMapper.ToEntity(request), cancellationToken);
        return CreatedAtAction(
            nameof(GetById),
            new { sourceId = created.SourceId, targetId = created.TargetId },
            ItsmContractMapper.ToResponse(created));
    }

    [HttpPut("{sourceId:guid}/{targetId:guid}")]
    public async Task<ActionResult<ConfigurationItemDependencyResponse>> Update(
        Guid sourceId,
        Guid targetId,
        [FromBody] ConfigurationItemDependencyRequest request,
        CancellationToken cancellationToken)
    {
        var dependency = ItsmContractMapper.ToEntity(request);
        dependency.SourceId = sourceId;
        dependency.TargetId = targetId;

        var updated = await service.UpdateAsync(dependency, cancellationToken);
        return updated is null ? NotFound() : Ok(ItsmContractMapper.ToResponse(updated));
    }

    [HttpDelete("{sourceId:guid}/{targetId:guid}")]
    public async Task<IActionResult> Delete(Guid sourceId, Guid targetId, CancellationToken cancellationToken)
    {
        var deleted = await service.DeleteAsync(sourceId, targetId, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
