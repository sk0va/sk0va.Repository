using Sample.App;
using Sample.Core;
using Skova.Repository.Impl;

namespace Sample.Db;

// EntitySpecification is a base class for specifications as well as Entity is the root base class for entities like Person
// Note, that we use parametrization with contstrant for typeparameters to Entities-derived classes for both layers domain and db.
// This is required to make all IQuery<> transformations to be typed with most specific entity type. We can'd just re-use SpecificationContainer<DbEntity> 
// in children classes
public class EntitySpecification<TEntity, TDbEntity> : IEntitySpecification, IQueryTransformer<TDbEntity>
    where TEntity : Entity
    where TDbEntity : DbEntity
{
    protected readonly SpecificationContainer<TDbEntity> _container = new();

    public void GetById(Guid id)
    {
        _container.AddTranformation(q => q.Where(p => p.Id == id));
    }

    public IQueryable<TDbEntity> Apply(IQueryable<TDbEntity> query)
    {
        return _container.Apply(query);
    }
}
