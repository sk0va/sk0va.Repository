using Microsoft.Extensions.DependencyInjection;
using Skova.Repository.Abstractions;
using Skova.Repository.DependencyInjection;

namespace Skova.Repository.Tests;

public class DependencyInjectionTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AllRegistrations_HappyPath(bool configRepositoryRegistration)
    {
        var sc = new ServiceCollection();

        sc.AddAutoMapper(GetType().Assembly);
        sc.AddDbContext<TestDbContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()));

        var uwContext = sc.AddUnitOfWorkAsScoped<TestDbContext>();

        var repoContext = configRepositoryRegistration
            ? uwContext.AddRepositoryAsScoped<Person, DbPerson>(c => c.AddKeyRecognizer(p => new object[] { p.Id }))
            : uwContext.AddRepositoryAsScoped<Person, DbPerson>().AddKeyRecognizer(p => new object[] { p.Id });

        repoContext
            .AddSpecificationAsTransient<IPersonSpecification, PersonSpecification>()
            .AddSpecificationAsTransient<ISpecification<Person>, PersonSpecification>();

        var provider = sc.BuildServiceProvider();

        using var scope = provider.CreateScope();

        scope.ServiceProvider.GetRequiredService<IUnitOfWork>().Should().NotBeNull();
        scope.ServiceProvider.GetRequiredService<IRepository<Person>>().Should().NotBeNull();
        scope.ServiceProvider.GetRequiredService<SpecificationFactory<IPersonSpecification>>().Should().NotBeNull();
        scope.ServiceProvider.GetRequiredService<ISpecification<Person>>().Should().NotBeNull();
    }
}