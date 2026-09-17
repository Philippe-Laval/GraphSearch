using GraphRag.ITSM.Entities;
using GraphRag.ITSM.Services;
using ITSM.ApiService.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace ITSM.ApiService.Controllers;

[ApiController]
[Route("api/service-level-agreements")]
public sealed class ServiceLevelAgreementsController(ServiceLevelAgreementService service)
    : EntityCrudControllerBase<ServiceLevelAgreement, ServiceLevelAgreementRequest, ServiceLevelAgreementResponse, ServiceLevelAgreementService>(
        service,
        ItsmContractMapper.ToEntity,
        ItsmContractMapper.ToResponse);

[ApiController]
[Route("api/sla-measurements")]
public sealed class SlaMeasurementsController(SlaMeasurementService service)
    : EntityCrudControllerBase<SlaMeasurement, SlaMeasurementRequest, SlaMeasurementResponse, SlaMeasurementService>(
        service,
        ItsmContractMapper.ToEntity,
        ItsmContractMapper.ToResponse);

[ApiController]
[Route("api/business-calendars")]
public sealed class BusinessCalendarsController(BusinessCalendarService service)
    : EntityCrudControllerBase<BusinessCalendar, BusinessCalendarRequest, BusinessCalendarResponse, BusinessCalendarService>(
        service,
        ItsmContractMapper.ToEntity,
        ItsmContractMapper.ToResponse);

[ApiController]
[Route("api/calendar-holidays")]
public sealed class CalendarHolidaysController(CalendarHolidayService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CalendarHolidayResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var holidays = await service.GetAllAsync(cancellationToken);
        return Ok(holidays.Select(ItsmContractMapper.ToResponse).ToArray());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CalendarHolidayResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var holiday = await service.GetByIdAsync(id, cancellationToken);
        return holiday is null ? NotFound() : Ok(ItsmContractMapper.ToResponse(holiday));
    }

    [HttpPost]
    public async Task<ActionResult<CalendarHolidayResponse>> Create([FromBody] CalendarHolidayRequest request, CancellationToken cancellationToken)
    {
        var created = await service.CreateAsync(ItsmContractMapper.ToEntity(request), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ItsmContractMapper.ToResponse(created));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CalendarHolidayResponse>> Update(Guid id, [FromBody] CalendarHolidayRequest request, CancellationToken cancellationToken)
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
