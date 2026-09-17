using GraphRag.ITSM.Services;
using ITSM.ApiService.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace ITSM.ApiService.Controllers;

[ApiController]
[Route("api/people")]
public sealed class PeopleController(PersonService service)
    : EntityCrudControllerBase<GraphRag.ITSM.Entities.Person, PersonRequest, PersonResponse, PersonService>(
        service,
        ItsmContractMapper.ToEntity,
        ItsmContractMapper.ToResponse);

[ApiController]
[Route("api/agents")]
public sealed class AgentsController(AgentService service)
    : EntityCrudControllerBase<GraphRag.ITSM.Entities.Agent, AgentRequest, AgentResponse, AgentService>(
        service,
        ItsmContractMapper.ToEntity,
        ItsmContractMapper.ToResponse);

[ApiController]
[Route("api/organizations")]
public sealed class OrganizationsController(OrganizationService service)
    : EntityCrudControllerBase<GraphRag.ITSM.Entities.Organization, OrganizationRequest, OrganizationResponse, OrganizationService>(
        service,
        ItsmContractMapper.ToEntity,
        ItsmContractMapper.ToResponse);

[ApiController]
[Route("api/assignment-groups")]
public sealed class AssignmentGroupsController(AssignmentGroupService service)
    : EntityCrudControllerBase<GraphRag.ITSM.Entities.AssignmentGroup, AssignmentGroupRequest, AssignmentGroupResponse, AssignmentGroupService>(
        service,
        ItsmContractMapper.ToEntity,
        ItsmContractMapper.ToResponse);

[ApiController]
[Route("api/vendors")]
public sealed class VendorsController(VendorService service)
    : EntityCrudControllerBase<GraphRag.ITSM.Entities.Vendor, VendorRequest, VendorResponse, VendorService>(
        service,
        ItsmContractMapper.ToEntity,
        ItsmContractMapper.ToResponse);

[ApiController]
[Route("api/agent-group-memberships")]
public sealed class AgentGroupMembershipsController(AgentGroupMembershipService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AgentGroupMembershipResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var memberships = await service.GetAllAsync(cancellationToken);
        return Ok(memberships.Select(ItsmContractMapper.ToResponse).ToArray());
    }

    [HttpGet("{agentId:guid}/{assignmentGroupId:guid}")]
    public async Task<ActionResult<AgentGroupMembershipResponse>> GetById(
        Guid agentId,
        Guid assignmentGroupId,
        CancellationToken cancellationToken)
    {
        var membership = await service.GetAsync(agentId, assignmentGroupId, cancellationToken);
        return membership is null ? NotFound() : Ok(ItsmContractMapper.ToResponse(membership));
    }

    [HttpPost]
    public async Task<ActionResult<AgentGroupMembershipResponse>> Create(
        [FromBody] AgentGroupMembershipRequest request,
        CancellationToken cancellationToken)
    {
        var created = await service.CreateAsync(ItsmContractMapper.ToEntity(request), cancellationToken);
        return CreatedAtAction(
            nameof(GetById),
            new { agentId = created.AgentId, assignmentGroupId = created.AssignmentGroupId },
            ItsmContractMapper.ToResponse(created));
    }

    [HttpPut("{agentId:guid}/{assignmentGroupId:guid}")]
    public async Task<ActionResult<AgentGroupMembershipResponse>> Update(
        Guid agentId,
        Guid assignmentGroupId,
        [FromBody] AgentGroupMembershipRequest request,
        CancellationToken cancellationToken)
    {
        var membership = ItsmContractMapper.ToEntity(request);
        membership.AgentId = agentId;
        membership.AssignmentGroupId = assignmentGroupId;

        var updated = await service.UpdateAsync(membership, cancellationToken);
        return updated is null ? NotFound() : Ok(ItsmContractMapper.ToResponse(updated));
    }

    [HttpDelete("{agentId:guid}/{assignmentGroupId:guid}")]
    public async Task<IActionResult> Delete(Guid agentId, Guid assignmentGroupId, CancellationToken cancellationToken)
    {
        var deleted = await service.DeleteAsync(agentId, assignmentGroupId, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
