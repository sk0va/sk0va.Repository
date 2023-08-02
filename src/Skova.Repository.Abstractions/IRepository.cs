using Skova.Repository.Abstractions.Specifications;

namespace Skova.Repository.Abstractions;

/// <summary>
/// Represent a repository for entities of TDomain type
/// <typeparamref name="TDomain">Domain type of entities to query. Implementations of this type should care about mapping between domain layer and underlying data layer<typeparamref>
/// </summary>
public interface IRepository<TDomain>
{
    /// <summary>
    /// Prepare a query to a storage by provided specification
    /// <param name="specification"></param>
    /// </summary>
    IEntityQuery<TDomain> With(ISpecification<TDomain> specification);

    /// <summary>
    /// Add entity to the repository. To apply changes to the storage, call <see cref="IUnitOfWork.SaveChangesAsync(CancellationToken)"/>
    /// <param name="entity">The entity to add</param>
    /// <param name="ct">CancellationToken which will be transitionally passed to underlying implementation logic </param>
    /// </summary>
    Task AddAsync(TDomain entity, CancellationToken ct);

    /// <summary>
    /// Update entity in repository. To apply changes to the storage, call <see cref="IUnitOfWork.SaveChangesAsync(CancellationToken)"/>
    /// <param name="entity">Entity to update</param>
    /// </summary>
    void Update(TDomain entity);

    /// <summary>
    /// Add entity to the repository. To apply changes to the storage, call <see cref="IUnitOfWork.SaveChangesAsync(CancellationToken)"/>
    /// <param name="entity">Entity to delete</param>
    /// </summary>
    void Delete(TDomain entity);
}
