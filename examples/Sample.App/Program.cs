using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sample.Db;
using Sample.Core;

using Skova.Repository.Abstractions.Specifications;
using Skova.Repository.Abstractions;
using Skova.Repository.Impl;

namespace Sample.App;

public class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        var builder = Host.CreateApplicationBuilder(args);

        var services = builder.Services;

        services.AddDbContext<CompanyDbContext>();
        services.AddHostedService<CompanyHostService>();

        services.AddAutoMapper(typeof(Program).Assembly);

        services.AddScoped<IUnitOfWork, UnitOfWork<CompanyDbContext>>();
        services.AddScoped<IRepository<Person>, GenericRepository<Person, DbPerson, CompanyDbContext>>();
        services.AddScoped<PersonService>();
        services.AddSingleton<SpecificationFactory<IPersonSpecification>>(sp => () => (IPersonSpecification)sp.GetRequiredService(typeof(PersonSpecification)));
        services.AddTransient<ISpecification<Person>, PersonSpecification>();

        var app = builder.Build();
        app.Run();
    }
}
