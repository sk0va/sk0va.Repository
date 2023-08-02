using System.Linq.Expressions;

namespace Skova.Repository.Abstractions;

/// <summary>
/// Represent set of entity updation rules
/// <typeparamref name="TDomain">Domain type of entities to query. Implementations of this type should care about mapping between domain layer and underlying data layer<typeparamref>
/// </summary>
public interface IEntityUpdater<TDomain>
{
    /// <summary>
    /// Sets the specified property of a target entity to the specified value
    /// <typeparam name="TValue">Type of </typeparam>
    /// <param name="property">Property to set</param>
    /// <param name="value">Value to set to the property</param>
    /// </summary>
    IEntityUpdater<TDomain> Set<TValue>(Expression<Func<TDomain, TValue>> property, TValue value);
}
