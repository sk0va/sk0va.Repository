using Sample.App;
using Sample.Core;

namespace Sample.Db;

public class PersonSpecification : EntitySpecification<Person, DbPerson>, IPersonSpecification

// In case when you need to create new specification classes which will inherits from PersonSpecification, 
// you should change it signature (and fix DI) to make it generic as follow:

// public class PersonSpecification <TPerson, TDbPerson> : EntitySpecification<TPerson, TDbPerson>, IPersonSpecification
//     where TPerson : Person    
//     where TDbPerson : DbPerson
{
    public void ByName(string name)
    {
        AddTranformation(q => q.Where(p => p.Name == name));
    }

    public void MinimalAge(int age)
    {
        AddTranformation(q => q.Where(p => p.Age >= age));
    }

    public void OrderByAge(bool descending = false)
    {
        AddTranformation(q => descending ? q.OrderByDescending(p => p.Age) : q.OrderBy(p => p.Age));
    }
}
