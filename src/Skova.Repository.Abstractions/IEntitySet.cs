namespace Skova.Repository.Abstractions;

public interface IEntitySet<T>
{
    Task<IList<T>> ToListAsync(CancellationToken ct);
    Task UpdateAsync(Func<IEntityUpdater<T>, IEntityUpdater<T>> configureEntityUpdater, CancellationToken ct);
    Task DeleteAllAsync(CancellationToken ct);
}
