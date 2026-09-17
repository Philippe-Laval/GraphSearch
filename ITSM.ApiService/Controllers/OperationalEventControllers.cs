using GraphRag.ITSM.Services;
using ITSM.ApiService.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace ITSM.ApiService.Controllers;

[ApiController]
[Route("api/status-transitions")]
public sealed class StatusTransitionsController(StatusTransitionService service)
    : EntityCrudControllerBase<GraphRag.ITSM.Entities.StatusTransition, StatusTransitionRequest, StatusTransitionResponse, StatusTransitionService>(
        service,
        ItsmContractMapper.ToEntity,
        ItsmContractMapper.ToResponse);

[ApiController]
[Route("api/assignment-events")]
public sealed class AssignmentEventsController(AssignmentEventService service)
    : EntityCrudControllerBase<GraphRag.ITSM.Entities.AssignmentEvent, AssignmentEventRequest, AssignmentEventResponse, AssignmentEventService>(
        service,
        ItsmContractMapper.ToEntity,
        ItsmContractMapper.ToResponse);

[ApiController]
[Route("api/escalation-events")]
public sealed class EscalationEventsController(EscalationEventService service)
    : EntityCrudControllerBase<GraphRag.ITSM.Entities.EscalationEvent, EscalationEventRequest, EscalationEventResponse, EscalationEventService>(
        service,
        ItsmContractMapper.ToEntity,
        ItsmContractMapper.ToResponse);

[ApiController]
[Route("api/reassignment-events")]
public sealed class ReassignmentEventsController(ReassignmentEventService service)
    : EntityCrudControllerBase<GraphRag.ITSM.Entities.ReassignmentEvent, ReassignmentEventRequest, ReassignmentEventResponse, ReassignmentEventService>(
        service,
        ItsmContractMapper.ToEntity,
        ItsmContractMapper.ToResponse);

[ApiController]
[Route("api/resolution-events")]
public sealed class ResolutionEventsController(ResolutionEventService service)
    : EntityCrudControllerBase<GraphRag.ITSM.Entities.ResolutionEvent, ResolutionEventRequest, ResolutionEventResponse, ResolutionEventService>(
        service,
        ItsmContractMapper.ToEntity,
        ItsmContractMapper.ToResponse);

[ApiController]
[Route("api/reopen-events")]
public sealed class ReopenEventsController(ReopenEventService service)
    : EntityCrudControllerBase<GraphRag.ITSM.Entities.ReopenEvent, ReopenEventRequest, ReopenEventResponse, ReopenEventService>(
        service,
        ItsmContractMapper.ToEntity,
        ItsmContractMapper.ToResponse);

[ApiController]
[Route("api/closure-events")]
public sealed class ClosureEventsController(ClosureEventService service)
    : EntityCrudControllerBase<GraphRag.ITSM.Entities.ClosureEvent, ClosureEventRequest, ClosureEventResponse, ClosureEventService>(
        service,
        ItsmContractMapper.ToEntity,
        ItsmContractMapper.ToResponse);
