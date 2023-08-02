namespace Skova.Repository.Abstractions;

/// <summary>
/// Represent a query logic to the data storage.
/// <typeparamref name="TDomain">Domain type of entities to query. Implementations of this type should care about mapping between domain layer and underlying data layer<typeparamref>
/// </summary>
public interface IEntityQuery<TDomain>
{
    /// <summary>
    /// Execute the query against the data storage and attach entities to the unit-of-work
    /// <param name="ct">CancellationToken which will be transitionally passed to underlying implementation logic </param>
    /// </summary>
    Task<IList<TDomain>> LoadAsync(CancellationToken ct);

    /// <summary>
    /// Execute the query against the data storage without attaching entities to the unit-of-work
    /// <param name="ct">CancellationToken which will be transitionally passed to underlying implementation logic </param>
    /// </summary>
    Task<IList<TDomain>> ExecuteQueryAsync(CancellationToken ct);

    /// <summary>
    /// Run entities update operation directly on a storage without loading entities to unit-of-work. This query will determine whcih entities should be 
    /// <param name="configureEntityUpdater">Use it to setup entity updation logic</param>
    /// <param name="ct">CancellationToken which will be transitionally passed to underlying implementation logic </param>
    /// </summary>
    Task ExecuteUpdateAsync(Func<IEntityUpdater<TDomain>, IEntityUpdater<TDomain>> configureEntityUpdater, CancellationToken ct);

    /// <summary>
    /// Run entities delete operation directly on a storage without loading entities to unit-of-work
    /// <param name="ct">CancellationToken which will be transitionally passed to underlying implementation logic </param>
    /// </summary>
    Task ExecuteDeleteAllAsync(CancellationToken ct);
}
