using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Skova.Repository.Abstractions;
using Skova.Repository.Impl;

namespace Skova.Repository.DependencyInjection;

public static class DependencyRegistrar
{
    public static UnitOfWorkRegistrationContext<TDbContext> AddUnitOfWorkAsScoped<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext
    {
        services.AddScoped<IUnitOfWork, UnitOfWork<TDbContext>>();
        return new UnitOfWorkRegistrationContext<TDbContext>(services);
    }
}