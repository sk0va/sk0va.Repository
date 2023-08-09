using Skova.Repository.Abstractions;
using Skova.Repository.Abstractions.Specifications;

namespace Sample.Core;

public class PersonService
{
    public PersonService(
        IUnitOfWork unitOfWork,
        IRepository<Person> repository,
        SpecificationFactory<IPersonSpecification> specificationFactory)
    {
        Repository = repository;
        UnitOfWork = unitOfWork;
        SpecificationFactory = specificationFactory;
    }

    public IRepository<Person> Repository { get; }
    public IUnitOfWork UnitOfWork { get; }
    public SpecificationFactory<IPersonSpecification> SpecificationFactory { get; }

    public async Task CreateAsync(Person person, CancellationToken ct = default)
    {
        await Repository.AddAsync(person, ct);
        await UnitOfWork.SaveChangesAsync(ct);
    }

    public async Task<Person> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var spec = SpecificationFactory();
        spec.GetById(id);

        var results = await Repository.With(spec).ExecuteQueryAsync(ct);
        return results.SingleOrDefault();
    }

    public async Task<IEnumerable<Person>> GetByAgeAndNameAsync(int personAge, string name, CancellationToken ct = default)
    {
        var spec = SpecificationFactory();
        
        spec.MinimalAge(personAge);
        spec.ByName(name);

        spec.OrderByAge();

        return await Repository.With(spec).ExecuteQueryAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var spec = SpecificationFactory();
        spec.GetById(id);

        await Repository.With(spec).ExecuteDeleteAllAsync(ct);
    }

    public async Task UpdateNameAsync(Guid id, string name, CancellationToken ct = default)
    {
        var spec = SpecificationFactory();
        spec.GetById(id);

        await Repository.With(spec).ExecuteUpdateAsync(p => p.Set(p => p.PersonName, name), ct);
    }
}