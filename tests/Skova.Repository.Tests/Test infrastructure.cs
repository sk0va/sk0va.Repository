global using Xunit;
global using FluentAssertions;
global using Skova.Repository.Impl;
global using Skova.Repository.Abstractions.Specifications;
global using Microsoft.EntityFrameworkCore;

namespace Skova.Repository.Tests;

public class DbPerson
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null;
    public string LastName { get; set; } = null;
    public string Email { get; set; }
}

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    public DbSet<DbPerson> People { get; set; } = null;
}

public class Person
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null;
    public string LastName { get; set; } = null;
    public string Email { get; set; }
}

public class PersonSpecification : ISpecification<Person>, IQueryTransformer<DbPerson>
{
    private readonly SpecificationContainer<DbPerson> _specificationContainer = new();
    
    public void ById(Guid id)
    {
        _specificationContainer.AddTranformation(q => q.Where(e => e.Id == id));
    }

    public IQueryable<DbPerson> Apply(IQueryable<DbPerson> query) => _specificationContainer.Apply(query);
}
