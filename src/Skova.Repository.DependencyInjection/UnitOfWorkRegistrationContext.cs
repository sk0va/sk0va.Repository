using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Skova.Repository.Abstractions;
using Skova.Repository.Impl;

namespace Skova.Repository.DependencyInjection;

public class UnitOfWorkRegistrationContext<TDbContext>
    where TDbContext : DbContext
{
    protected IServiceCollection Services { get; }

    public UnitOfWorkRegistrationContext(IServiceCollection services)
    {
        Services = services;
    }

    public RepositoryRegistrationContext<TEntity, TDb, TDbContext>
        AddRepositoryAsScoped<TEntity, TDb>(
             Func<RepositoryRegistrationContext<TEntity, TDb, TDbContext>,
                  RepositoryRegistrationContext<TEntity, TDb, TDbContext>> config = null)
                where TDb : class
    {
        Services.AddScoped<IRepository<TEntity>, GenericRepository<TEntity, TDb, TDbContext>>();

        RepositoryRegistrationContext<TEntity, TDb, TDbContext> res = new (Services);
        res = config?.Invoke(res) ?? res;
        return res;
    }
}
