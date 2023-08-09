namespace Skova.Repository.Abstractions.Specifications;

/// <summary>
/// Function to detemine Database ID value for domain entity
/// </summary>
public delegate object[] KeyRecognizer<TDomain>(TDomain entity);