using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Skova.Repository.Abstractions.Specifications;

namespace Skova.Repository.DependencyInjection;

public record class RepositoryRegistrationContext<TEntity, TDb, TDbContext> : RegistrationContext<TDbContext>
    where TDbContext : DbContext
    where TDb : class
{
    public RepositoryRegistrationContext(IServiceCollection Services) : base(Services)
    {
    }

    public RepositoryRegistrationContext<TEntity, TDb, TDbContext>
        AddSpecificationAsTransient<TSpecAbstraction, TSpec>()
                where TSpecAbstraction : ISpecification<TEntity>
                where TSpec : class, TSpecAbstraction, ISpeicificationBase
    {
        Services.AddTransient<TSpec>();
        Services.AddTransient<SpecificationFactory<TSpecAbstraction>>(
            sp => () => (TSpecAbstraction)sp.GetRequiredService(typeof(TSpec)));
        Services.AddTransient<ISpecification<TEntity>, TSpec>();

        return this;
    }
}
