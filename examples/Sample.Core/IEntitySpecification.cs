using Skova.Repository.Abstractions.Specifications;

namespace Sample.Core;

public interface IEntitySpecification : ISpecification<Entity>
{
    public void GetById(Guid id);
}