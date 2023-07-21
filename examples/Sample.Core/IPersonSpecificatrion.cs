using Skova.Repository.Abstractions.Specifications;

namespace Sample.Core;

public interface IPersonSpecification : ISpecification<Person>
{
    public void GetById(Guid id);
    public void GetByName(string name);
    public void GetByAge(int age);
}