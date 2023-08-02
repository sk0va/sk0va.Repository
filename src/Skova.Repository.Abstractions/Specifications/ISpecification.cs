namespace Skova.Repository.Abstractions.Specifications;

/// <summary>
/// Typed specification
/// <typeparamref name="TDomain">Domain type of entities to query. Implementations of this type should care about mapping between domain layer and underlying data layer<typeparamref>
/// </summary>
public interface ISpecification<out TDomain> : ISpeicificationBase
{
}
