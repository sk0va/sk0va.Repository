using Skova.Repository.Abstractions.Specifications;

namespace Skova.Repository.Abstractions;

public interface IRepository<T>
{
    IEntitySet<T> With(ISpecification<T> specification);

    Task<T> GetByIdAsync(Guid id, CancellationToken ct);

    Task AddAsync(T entity, CancellationToken ct);

    void Update(T entity);

    Task DeleteAsync(Guid id, CancellationToken ct);
}
