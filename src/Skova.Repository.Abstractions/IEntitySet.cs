namespace Skova.Repository.Abstractions;

public interface IEntitySet<T>
{
    Task UpdateAsync(Func<IEntityUpdater<T>, IEntityUpdater<T>> configureEntityUpdater, CancellationToken ct);

    Task DeleteAllAsync(CancellationToken ct);

    Task<IList<T>> ToListAsync(CancellationToken ct);
}
