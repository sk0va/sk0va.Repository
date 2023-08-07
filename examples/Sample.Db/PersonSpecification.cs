using Sample.App;
using Sample.Core;

namespace Sample.Db;

public class PersonSpecification : EntitySpecification<Person, DbPerson>, IPersonSpecification

// In case when you need to create new specification classes which will inherits from PersonSpecification, 
// you should change it signature (and fix DI) to make it generic as follow:

// public class PersonSpecification <TPerson, TDbPerson> : EntitySpecification<TPerson, TDbPerson>, IPersonSpecification
//     where TPerson : Person    
//     where TDbPerson: DbPerson
{
    public void GetByName(string name)
    {
        _container.AddTranformation(q => q.Where(p => p.Name == name));
    }

    public void GetByAge(int age)
    {
        _container.AddTranformation(q => q.Where(p => p.Age == age));
    }
}
