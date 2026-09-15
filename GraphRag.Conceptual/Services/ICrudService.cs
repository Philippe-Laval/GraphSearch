using GraphRag.Conceptual.Entities;

namespace GraphRag.Conceptual.Services;

/// <summary>
/// Generic CRUD contract for entities that inherit from <see cref="ConceptualEntity"/>.
/// </summary>
public interface ICrudService<T> where T : ConceptualEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<T?> GetByUriAsync(string uri, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> ListAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default);
    Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default);
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
}
