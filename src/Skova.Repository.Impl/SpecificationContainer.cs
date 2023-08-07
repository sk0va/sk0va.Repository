using Skova.Repository.Abstractions.Specifications;

namespace Skova.Repository.Impl;

/// <summary>
/// Container for query transformations
/// </summary>
public class SpecificationContainer<TDb> : IQueryTransformer<TDb>
{
    private readonly List<Func<IQueryable<TDb>, IQueryable<TDb>>> _transformations = new();

    /// <summary>
    /// Add a transformation to the container
    /// </summary>    
    public void AddTranformation(Func<IQueryable<TDb>, IQueryable<TDb>> transformation)
    {
        _transformations.Add(transformation);
    }

    /// <inheritdoc/>
    public IQueryable<TDb> Apply(IQueryable<TDb> query)
    {
        foreach (var transformation in _transformations)
        {
            query = transformation(query);
        }

        return query;
    }
}
