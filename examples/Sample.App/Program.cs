﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sample.Db;
using Sample.Core;
using Skova.Repository.DependencyInjection;
using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.EntityFrameworkCore;

namespace Sample.App;

public class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        var builder = Host.CreateApplicationBuilder(args);

        var services = builder.Services;

        services.AddDbContext<CompanyDbContext>(
            c => c.UseNpgsql("Host=localhost;Database=example;Username=postgres;Password=postgres;Port=5432"));

        services.AddHostedService<CompanyHostService>();

        // WARNING! Using expression mapping in Automapper is reqired for proper expression translations!
        services.AddAutoMapper(
            c => c.AddExpressionMapping(),
            typeof(Program).Assembly);

        services.AddScoped<PersonService>();

        services.AddUnitOfWorkAsScoped<CompanyDbContext>()
            .AddRepositoryAsScoped<Person, DbPerson>()
            .AddSpecificationAsTransient<IPersonSpecification, PersonSpecification>();

        // Alternative way:

        // services.AddUnitOfWorkAsScoped<CompanyDbContext>()
        //     .AddRepositoryAsScoped<Person, DbPerson>(c =>
        //         c.AddSpecificationAsTransient<IPersonSpecification, PersonSpecification>());

        // Manual dependency injection setup will look like this:

        // services.AddScoped<IUnitOfWork, UnitOfWork<CompanyDbContext>>();
        // services.AddScoped<IRepository<Person>, GenericRepository<Person, DbPerson, CompanyDbContext>>();
        // services.AddTransient<PersonSpecification>();
        // services.AddTransient<ISpecification<Person>, PersonSpecification>();
        // services.AddTransient<SpecificationFactory<IPersonSpecification>>(
        //     sp => () => (IPersonSpecification)sp.GetRequiredService(typeof(PersonSpecification)));

        var app = builder.Build();
        app.Run();
        app.StopAsync().GetAwaiter().GetResult();
    }
}
