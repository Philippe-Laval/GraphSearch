using GraphRag.ITSM.Entities;
using GraphRag.ITSM.Services;
using Microsoft.AspNetCore.Mvc;

namespace ITSM.ApiService.Controllers;

/// <summary>
/// Shared CRUD controller for ITSM entities backed by <see cref="EntityService{TEntity}"/>.
/// </summary>
public abstract class EntityCrudControllerBase<TEntity, TRequest, TResponse, TService>(
    TService service,
    Func<TRequest, TEntity> toEntity,
    Func<TEntity, TResponse> toResponse) : ControllerBase
    where TEntity : EntityBase
    where TService : EntityService<TEntity>
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await service.GetAllAsync(cancellationToken);
        return Ok(entities.Select(toResponse).ToArray());
    }

    [HttpGet("paged")]
    public async Task<ActionResult<IReadOnlyList<TResponse>>> GetPage(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 100,
        CancellationToken cancellationToken = default)
    {
        var entities = await service.ListAsync(skip, take, cancellationToken);
        return Ok(entities.Select(toResponse).ToArray());
    }

    [HttpGet("count")]
    public async Task<ActionResult<int>> Count(CancellationToken cancellationToken)
    {
        var count = await service.CountAsync(cancellationToken);
        return Ok(count);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var entity = await service.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : Ok(toResponse(entity));
    }

    [HttpPost]
    public async Task<ActionResult<TResponse>> Create([FromBody] TRequest request, CancellationToken cancellationToken)
    {
        var created = await service.CreateAsync(toEntity(request), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, toResponse(created));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TResponse>> Update(Guid id, [FromBody] TRequest request, CancellationToken cancellationToken)
    {
        var updated = await service.UpdateAsync(id, toEntity(request), cancellationToken);
        return updated is null ? NotFound() : Ok(toResponse(updated));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await service.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
