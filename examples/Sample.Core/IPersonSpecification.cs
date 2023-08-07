using Skova.Repository.Abstractions.Specifications;

namespace Sample.Core;

public interface IPersonSpecification : ISpecification<Person>, IEntitySpecification
{
    public void GetByName(string name);
    public void GetByAge(int age);
}
