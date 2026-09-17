using GraphRag.ITSM.Entities;
using GraphRag.ITSM.Services;
using ITSM.ApiService.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace ITSM.ApiService.Controllers;

[ApiController]
[Route("api/incidents")]
public sealed class IncidentsController(IncidentService service)
    : EntityCrudControllerBase<Incident, IncidentRequest, IncidentResponse, IncidentService>(
        service,
        ItsmContractMapper.ToEntity,
        ItsmContractMapper.ToResponse);

[ApiController]
[Route("api/service-requests")]
public sealed class ServiceRequestsController(ServiceRequestService service)
    : EntityCrudControllerBase<ServiceRequest, ServiceRequestRequest, ServiceRequestResponse, ServiceRequestService>(
        service,
        ItsmContractMapper.ToEntity,
        ItsmContractMapper.ToResponse);

[ApiController]
[Route("api/problems")]
public sealed class ProblemsController(ProblemService service)
    : EntityCrudControllerBase<Problem, ProblemRequest, ProblemResponse, ProblemService>(
        service,
        ItsmContractMapper.ToEntity,
        ItsmContractMapper.ToResponse);

[ApiController]
[Route("api/change-requests")]
public sealed class ChangeRequestsController(ChangeRequestService service)
    : EntityCrudControllerBase<ChangeRequest, ChangeRequestRequest, ChangeRequestResponse, ChangeRequestService>(
        service,
        ItsmContractMapper.ToEntity,
        ItsmContractMapper.ToResponse);

[ApiController]
[Route("api/ticket-tasks")]
public sealed class TicketTasksController(TicketTaskService service)
    : EntityCrudControllerBase<TicketTask, TicketTaskRequest, TicketTaskResponse, TicketTaskService>(
        service,
        ItsmContractMapper.ToEntity,
        ItsmContractMapper.ToResponse);

[ApiController]
[Route("api/ticket-affected-cis")]
public sealed class TicketAffectedConfigurationItemsController(TicketAffectedCIService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TicketAffectedCIResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var affectedItems = await service.GetAllAsync(cancellationToken);
        return Ok(affectedItems.Select(ItsmContractMapper.ToResponse).ToArray());
    }

    [HttpGet("{ticketId:guid}/{configurationItemId:guid}")]
    public async Task<ActionResult<TicketAffectedCIResponse>> GetById(
        Guid ticketId,
        Guid configurationItemId,
        CancellationToken cancellationToken)
    {
        var affectedItem = await service.GetAsync(ticketId, configurationItemId, cancellationToken);
        return affectedItem is null ? NotFound() : Ok(ItsmContractMapper.ToResponse(affectedItem));
    }

    [HttpPost]
    public async Task<ActionResult<TicketAffectedCIResponse>> Create(
        [FromBody] TicketAffectedCIRequest request,
        CancellationToken cancellationToken)
    {
        var created = await service.CreateAsync(ItsmContractMapper.ToEntity(request), cancellationToken);
        return CreatedAtAction(
            nameof(GetById),
            new { ticketId = created.TicketId, configurationItemId = created.ConfigurationItemId },
            ItsmContractMapper.ToResponse(created));
    }

    [HttpPut("{ticketId:guid}/{configurationItemId:guid}")]
    public async Task<ActionResult<TicketAffectedCIResponse>> Update(
        Guid ticketId,
        Guid configurationItemId,
        [FromBody] TicketAffectedCIRequest request,
        CancellationToken cancellationToken)
    {
        var affectedItem = ItsmContractMapper.ToEntity(request);
        affectedItem.TicketId = ticketId;
        affectedItem.ConfigurationItemId = configurationItemId;

        var updated = await service.UpdateAsync(affectedItem, cancellationToken);
        return updated is null ? NotFound() : Ok(ItsmContractMapper.ToResponse(updated));
    }

    [HttpDelete("{ticketId:guid}/{configurationItemId:guid}")]
    public async Task<IActionResult> Delete(Guid ticketId, Guid configurationItemId, CancellationToken cancellationToken)
    {
        var deleted = await service.DeleteAsync(ticketId, configurationItemId, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}

[ApiController]
[Route("api/ticket-affected-services")]
public sealed class TicketAffectedServicesController(TicketAffectedServiceService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TicketAffectedServiceResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var affectedServices = await service.GetAllAsync(cancellationToken);
        return Ok(affectedServices.Select(ItsmContractMapper.ToResponse).ToArray());
    }

    [HttpGet("{ticketId:guid}/{serviceId:guid}")]
    public async Task<ActionResult<TicketAffectedServiceResponse>> GetById(
        Guid ticketId,
        Guid serviceId,
        CancellationToken cancellationToken)
    {
        var affectedService = await service.GetAsync(ticketId, serviceId, cancellationToken);
        return affectedService is null ? NotFound() : Ok(ItsmContractMapper.ToResponse(affectedService));
    }

    [HttpPost]
    public async Task<ActionResult<TicketAffectedServiceResponse>> Create(
        [FromBody] TicketAffectedServiceRequest request,
        CancellationToken cancellationToken)
    {
        var created = await service.CreateAsync(ItsmContractMapper.ToEntity(request), cancellationToken);
        return CreatedAtAction(
            nameof(GetById),
            new { ticketId = created.TicketId, serviceId = created.ServiceId },
            ItsmContractMapper.ToResponse(created));
    }

    [HttpPut("{ticketId:guid}/{serviceId:guid}")]
    public async Task<ActionResult<TicketAffectedServiceResponse>> Update(
        Guid ticketId,
        Guid serviceId,
        [FromBody] TicketAffectedServiceRequest request,
        CancellationToken cancellationToken)
    {
        var affectedService = ItsmContractMapper.ToEntity(request);
        affectedService.TicketId = ticketId;
        affectedService.ServiceId = serviceId;

        var updated = await service.UpdateAsync(affectedService, cancellationToken);
        return updated is null ? NotFound() : Ok(ItsmContractMapper.ToResponse(updated));
    }

    [HttpDelete("{ticketId:guid}/{serviceId:guid}")]
    public async Task<IActionResult> Delete(Guid ticketId, Guid serviceId, CancellationToken cancellationToken)
    {
        var deleted = await service.DeleteAsync(ticketId, serviceId, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}

[ApiController]
[Route("api/ticket-relations")]
public sealed class TicketRelationsController(TicketRelationService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TicketRelationResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var relations = await service.GetAllAsync(cancellationToken);
        return Ok(relations.Select(ItsmContractMapper.ToResponse).ToArray());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TicketRelationResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var relation = await service.GetByIdAsync(id, cancellationToken);
        return relation is null ? NotFound() : Ok(ItsmContractMapper.ToResponse(relation));
    }

    [HttpPost]
    public async Task<ActionResult<TicketRelationResponse>> Create([FromBody] TicketRelationRequest request, CancellationToken cancellationToken)
    {
        var created = await service.CreateAsync(ItsmContractMapper.ToEntity(request), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ItsmContractMapper.ToResponse(created));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TicketRelationResponse>> Update(Guid id, [FromBody] TicketRelationRequest request, CancellationToken cancellationToken)
    {
        var updated = await service.UpdateAsync(id, ItsmContractMapper.ToEntity(request), cancellationToken);
        return updated is null ? NotFound() : Ok(ItsmContractMapper.ToResponse(updated));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await service.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
