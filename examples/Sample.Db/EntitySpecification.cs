using Sample.App;
using Sample.Core;
using Skova.Repository.Impl;

namespace Sample.Db;

// EntitySpecification is a base class for specifications as well as Entity is the root base class for entities like Person
// Note, that we use parametrization with constraint for typeparameters to Entities-derived classes for both layers domain and db.
// This is required to make all IQuery<> transformations to be typed with most specific entity type. We can't just re-use SpecificationContainer<DbEntity> 
// in children classes
public class EntitySpecification<TDomain, TDb> : QueryTransformationsContainer<TDb>, IEntitySpecification
    where TDomain : Entity
    where TDb : DbEntity
{
    public void GetById(Guid id)
    {
        AddTranformation(q => q.Where(p => p.Id == id));
    }
}
