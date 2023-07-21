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

    public async Task AddAsync(Person person, CancellationToken ct = default)
    {
        await Repository.AddAsync(person, ct);
        await UnitOfWork.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<Person>> GetByAgeAsync(int personAge, CancellationToken ct)
    {
        var spec = SpecificationFactory();
        spec.GetByAge(personAge);

        return await Repository
            .With(spec)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Person>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var spec = SpecificationFactory();
        spec.GetById(id);

        return await Repository
            .With(spec)
            .ToListAsync(ct);
    }
}