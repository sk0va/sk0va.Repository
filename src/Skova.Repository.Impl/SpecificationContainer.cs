using Skova.Repository.Abstractions.Specifications;

namespace Skova.Repository.Impl;

public class SpecificationContainer<TDomain, TDb> : ISpecification<TDomain>, IQueryTransformer<TDb>
{
    private readonly List<Func<IQueryable<TDb>, IQueryable<TDb>>> _transformations = new();

    public void AddTranformation(Func<IQueryable<TDb>, IQueryable<TDb>> transformation)
    {
        _transformations.Add(transformation);
    }

    public IQueryable<TDb> Apply(IQueryable<TDb> query)
    {
        foreach (var transformation in _transformations)
        {
            query = transformation(query);
        }

        return query;
    }
}
