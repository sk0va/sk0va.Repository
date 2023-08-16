using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Skova.Repository.Abstractions;
using Skova.Repository.Impl;

namespace Skova.Repository.DependencyInjection;

public record class UnitOfWorkRegistrationContext<TDbContext>(IServiceCollection Services)
    where TDbContext : DbContext
{
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
