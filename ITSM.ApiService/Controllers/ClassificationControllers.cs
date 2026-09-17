using GraphRag.ITSM.Services;
using ITSM.ApiService.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace ITSM.ApiService.Controllers;

[ApiController]
[Route("api/categories")]
public sealed class CategoriesController(CategoryService service)
    : EntityCrudControllerBase<GraphRag.ITSM.Entities.Category, CategoryRequest, CategoryResponse, CategoryService>(
        service,
        ItsmContractMapper.ToEntity,
        ItsmContractMapper.ToResponse);

[ApiController]
[Route("api/statuses")]
public sealed class StatusesController(StatusService service)
    : EntityCrudControllerBase<GraphRag.ITSM.Entities.Status, StatusRequest, StatusResponse, StatusService>(
        service,
        ItsmContractMapper.ToEntity,
        ItsmContractMapper.ToResponse);

[ApiController]
[Route("api/impacts")]
public sealed class ImpactsController(ImpactService service)
    : EntityCrudControllerBase<GraphRag.ITSM.Entities.Impact, ImpactRequest, ImpactResponse, ImpactService>(
        service,
        ItsmContractMapper.ToEntity,
        ItsmContractMapper.ToResponse);

[ApiController]
[Route("api/urgencies")]
public sealed class UrgenciesController(UrgencyService service)
    : EntityCrudControllerBase<GraphRag.ITSM.Entities.Urgency, UrgencyRequest, UrgencyResponse, UrgencyService>(
        service,
        ItsmContractMapper.ToEntity,
        ItsmContractMapper.ToResponse);

[ApiController]
[Route("api/priorities")]
public sealed class PrioritiesController(PriorityService service)
    : EntityCrudControllerBase<GraphRag.ITSM.Entities.Priority, PriorityRequest, PriorityResponse, PriorityService>(
        service,
        ItsmContractMapper.ToEntity,
        ItsmContractMapper.ToResponse);

[ApiController]
[Route("api/severities")]
public sealed class SeveritiesController(SeverityService service)
    : EntityCrudControllerBase<GraphRag.ITSM.Entities.Severity, SeverityRequest, SeverityResponse, SeverityService>(
        service,
        ItsmContractMapper.ToEntity,
        ItsmContractMapper.ToResponse);
