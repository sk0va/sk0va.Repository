namespace Skova.Repository.Abstractions.Specifications;

public delegate TSpec SpecificationFactory<TSpec, T>() where TSpec : ISpecification<T>;
