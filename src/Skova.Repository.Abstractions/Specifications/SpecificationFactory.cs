namespace Skova.Repository.Abstractions.Specifications;

/// <summary>
/// Factory for specifications
/// </summary>
public delegate TSpec SpecificationFactory<TSpec>() where TSpec : ISpeicificationBase;
