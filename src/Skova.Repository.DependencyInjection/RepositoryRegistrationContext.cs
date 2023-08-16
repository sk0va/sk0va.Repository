using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Skova.Repository.Abstractions.Specifications;
using Skova.Repository.Impl;

namespace Skova.Repository.DependencyInjection;

public record class RepositoryRegistrationContext<TEntity, TDb, TDbContext> : UnitOfWorkRegistrationContext<TDbContext>
    where TDbContext : DbContext
    where TDb : class
{
    public RepositoryRegistrationContext(IServiceCollection Services) : base(Services)
    {
    }

    public RepositoryRegistrationContext<TEntity, TDb, TDbContext>
        AddSpecificationAsTransient<TSpecAbstraction, TSpec>()
                where TSpecAbstraction : ISpeicificationBase, ISpecification<TEntity>
                where TSpec : class, TSpecAbstraction, IQueryTransformer<TDb>
    {
        Services.AddTransient<TSpec>();
        Services.AddTransient<SpecificationFactory<TSpecAbstraction>>(
            sp => () => (TSpecAbstraction)sp.GetRequiredService(typeof(TSpec)));
        Services.AddTransient<ISpecification<TEntity>, TSpec>();

        return this;
    }

    public RepositoryRegistrationContext<TEntity, TDb, TDbContext> AddKeyRecognizer(Expression<KeyRecognizer<TEntity>> keyRecognizer) 
    {
        var recognizer = keyRecognizer.Compile();
        Services.AddSingleton(recognizer);
        return this;
    }
}
