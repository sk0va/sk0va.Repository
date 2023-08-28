using Skova.Repository.Abstractions.Specifications;

namespace Sample.Core;

public interface IPersonSpecification : ISpecification<Person>, IEntitySpecification
{
    void ByName(string name);
    void MinimalAge(int age);

    void OrderByAge(bool descending = false);

    void IncludeJobs();
}
