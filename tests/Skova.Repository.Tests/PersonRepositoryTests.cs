using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.EntityFrameworkCore.Internal;

namespace Skova.Repository.Tests;

public class PersonRepositoryTests 
{
    private readonly IMapper _mapper;
    private readonly TestDbContext _dbContext;
    private readonly UnitOfWork<TestDbContext> _unitOfWork;
    private readonly GenericRepository<Person, DbPerson, TestDbContext> _repo;
    private readonly Person _person;

    public PersonRepositoryTests()
    {
        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Person, DbPerson>();
            cfg.CreateMap<DbPerson, Person>();
            cfg.AddExpressionMapping();
        }).CreateMapper();

        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite("DataSource=file::memory:?cache=shared")
            .Options;

        _dbContext = new TestDbContext(options);
        _unitOfWork = new UnitOfWork<TestDbContext>(_dbContext);
        _repo = new GenericRepository<Person, DbPerson, TestDbContext>(_mapper, _dbContext, p => new object[] { p.Id });
        _dbContext.Database.EnsureCreated();

        _person = new Person
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = ""
        };
    }

    [Fact]
    public async Task AddAsync_HappyPath()
    {
        // act
        await _repo.AddAsync(_person);
     
        // assert
        var entry = _dbContext.ChangeTracker
            .Entries<DbPerson>()
            .FirstOrDefault(e => e.Entity.Id == _person.Id);

        entry.Should().NotBeNull();
        entry.State.Should().Be(EntityState.Added);
    }

    [Fact] 
    public async Task ExecuteQueryAsync_HappyPath()
    {
        await _repo.AddAsync(_person);
        await _dbContext.SaveChangesAsync();

        var spec = new PersonSpecification();
        spec.ById(_person.Id);
        
        // act
        var results = await _repo.With(spec).ExecuteQueryAsync();

        // assert
        results.Should().ContainSingle();
        var actual = results.Single();
        actual.FirstName.Should().Be(_person.FirstName);
        actual.LastName.Should().Be(_person.LastName);
        actual.Email.Should().Be(_person.Email);
    }

    [Fact] 
    public async Task LoadAsync_HappyPath()
    {
        await _repo.AddAsync(_person);
        await _dbContext.SaveChangesAsync();

        var spec = new PersonSpecification();
        spec.ById(_person.Id);
        
        // act
        var results = await _repo.With(spec).LoadAsync(default);

        // assert
        var entities = _dbContext.ChangeTracker.Entries<DbPerson>();
        entities.Should().BeEquivalentTo(results, options => options.ExcludingMissingMembers());
        
        results.Should().ContainSingle();
        var actual = results.Single();
        actual.FirstName.Should().Be(_person.FirstName);
        actual.LastName.Should().Be(_person.LastName);
        actual.Email.Should().Be(_person.Email);
    }

    [Fact] 
    public async Task Update_HappyPath()
    {
        await _repo.AddAsync(_person);
        await _dbContext.SaveChangesAsync();

        var spec = new PersonSpecification();
        spec.ById(_person.Id);
        
        // act
        _person.FirstName = "Jane";        
        _repo.Update(_person);
        await _unitOfWork.SaveChangesAsync();

        // assert
        var results = await _repo.With(spec).ExecuteQueryAsync();
        results.Should().ContainSingle();
        var actual = results.Single();
        actual.FirstName.Should().Be(_person.FirstName);
        actual.LastName.Should().Be(_person.LastName);
        actual.Email.Should().Be(_person.Email);
    }

    [Fact] 
    public async Task Delete_UnknownId_ThrowException()
    {
        await _repo.AddAsync(_person);
        await _dbContext.SaveChangesAsync();

        var spec = new PersonSpecification();
        spec.ById(_person.Id);
        
        // act
        Assert.Throws<KeyNotFoundException>(() => _repo.Delete(new Person { Id = Guid.NewGuid() }));
    }
    [Fact] 
    public async Task Update_UnknownId_ThrowException()
    {
        await _repo.AddAsync(_person);
        await _dbContext.SaveChangesAsync();

        var spec = new PersonSpecification();
        spec.ById(_person.Id);
        
        // act
        _person.Id = Guid.NewGuid();
        Assert.Throws<KeyNotFoundException>(() => _repo.Update(_person));
    }

    [Fact] 
    public async Task Delete_HappyPath()
    {
        await _repo.AddAsync(_person);
        await _dbContext.SaveChangesAsync();

        var spec = new PersonSpecification();
        spec.ById(_person.Id);
        
        // act
        _repo.Delete(new Person { Id = _person.Id });
        await _unitOfWork.SaveChangesAsync();

        // assert
        var results = await _repo.With(spec).ExecuteQueryAsync();
        results.Should().BeEmpty();
    }
        
    [Fact] 
    public async Task ExecuteDeleteAllAsync_HappyPath()
    {
        await _repo.AddAsync(_person);
        await _dbContext.SaveChangesAsync();
        
        var spec = new PersonSpecification();
        spec.ById(_person.Id);

        // act
        await _repo.With(spec).ExecuteDeleteAllAsync();

        // assert
        var results = await _repo.With(spec).ExecuteQueryAsync();
        results.Should().BeEmpty();
    }
        
    [Fact] 
    public async Task ExecuteUpdateAsync_HappyPath()
    {
        await _repo.AddAsync(_person);
        await _dbContext.SaveChangesAsync();
        
        var spec = new PersonSpecification();
        spec.ById(_person.Id);

        // act
        await _repo.With(spec)
            .ExecuteUpdateAsync(u => u.Set(x => x.FirstName, "Jane"));

        // assert
        var results = await _repo.With(spec).ExecuteQueryAsync();

        results.Should().ContainSingle();
        var actual = results.Single();
        actual.FirstName.Should().Be("Jane");
        actual.LastName.Should().Be(_person.LastName);
    }
}
