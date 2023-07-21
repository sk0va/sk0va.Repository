namespace Skova.Repository.Abstractions.Specifications;

public delegate TSpec SpecificationFactory<TSpec>() where TSpec : ISpeicificationBase;
