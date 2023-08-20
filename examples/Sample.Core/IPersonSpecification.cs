using Skova.Repository.Abstractions.Specifications;

namespace Sample.Core;

public interface IPersonSpecification : ISpecification<Person>, IEntitySpecification
{
    public void ByName(string name);
    public void MinimalAge(int age);

    public void OrderByAge(bool descending = false);
}
