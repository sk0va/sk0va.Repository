using Skova.Repository.Abstractions.Specifications;

namespace Sample.Core;

public interface IEntitySpecification : ISpecification<Entity>
{
    void GetById(Guid id);
}