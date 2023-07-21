using Sample.App;
using Sample.Core;
using Skova.Repository.Impl;

namespace Sample.Db;

public class PersonSpecification : IPersonSpecification, IQueryTransformer<DbPerson>
{
    private readonly SpecificationContainer<Person, DbPerson> _container = new();

    public void GetById(Guid id)
    {
        _container.AddTranformation(q => q.Where(p => p.Id == id));
    }

    public void GetByName(string name)
    {
        _container.AddTranformation(q => q.Where(p => p.Name == name));
    }

    public IQueryable<DbPerson> Apply(IQueryable<DbPerson> query)
    {
        return _container.Apply(query);
    }

    public void GetByAge(int age)
    {
        _container.AddTranformation(q => q.Where(p => p.Age == age));
    }
}
