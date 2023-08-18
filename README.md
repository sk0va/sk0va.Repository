

**IMPORTANT** Skova.Repositoty.* packages are not present in the NuGet repository at the moment and will be published soon. Follow the  updates

Use this library to implement the Repository pattern. Designed with Clean Architecture in mind. Hereafter the terminology from a Clean Architecture will be used.
It consists of 3 packages:

- **Skova.Repositoty.Abstractions**: This package contains base repository abstractions. Use it in your Application layer. Has no dependencies.
```
dotnet add package Skova.Repositoty.Abstractions
```

- **Skova.Repository.Impl**: This package depends on Skova.Repository.Abstractions, Automapper and EF Core. Use this package in your Persistent layer to implement specifications logic.
```
dotnet add package Skova.Repository.Impl
```

- **Skova.Repository.DependencyInjection**: Provides API for dependencies registrations through IServiceCollection. Use this package to register dependencies. This package 
```cli
dotnet add package Skova.Repository.DependencyInjection
```

## Usage

### Application Layer
In your Application layer, use the following abstractions:

- **`IUnitOfWork`** - represents unit-of-work that will be used to manage entities. UnitOfWork is usually registered in DI containers as scoped associated with some business transaction. Skova.Repository.Impl contains default implementation `UnitOfWork` which just wraps your db context classes. 

- **`IRepository<TDomain>`** - represents a repository for a domain layer's entity of the specified `TDomain` type. Supports operations for creating updating and deleting entities from a unit of work using `AddAsync`, `Update` and `Delete` methods. To load entities from a storage to unit-of-work or get them from storage without attaching to the unit-of-work, you should call method `With` and provide a specification of the entities you want to receive. Also, it supports the updation and deletion of entities that fit provided specification directly in storage like EF Core do (See "[ExecuteUpdate and ExecuteDelete](https://learn.microsoft.com/en-us/ef/core/saving/execute-insert-update-delete)" article in the MSDN documentation for details). You should use the method `With` first and provide specifications that will be used to define entities you want to update or delete.

- **`ISpecification<TDomain>`** - represents a specification for a domain layer's entity of the specified `TDomain` type. Specifications are used to customize queries when you want to load data from storage. This library doesn't strict developers to use some API for specifications and leaves it up to you how to design specifications API.

Sample specification may look like:
```csharp
public interface IPersonSpecification : ISpecification<Person>
{
    public void GetById(Guid id);
    public void ByName(string name, bool exactMatch = false);
    public void MinimalAge(int age);

    public void OrderByAge(bool descending = false);
}
```

Also, you may organize some of specifications into hierarchy:

```csharp
public interface IPersonSpecification : ISpecification<Person>, IEntitySpecification
{
    public void ByName(string name, bool exactMatch = false);
    public void MinimalAge(int age);

    public void OrderByAge(bool descending = false);
}

public interface IEntitySpecification : ISpecification<Entity>
{
    public void GetById(Guid id);
}
```

### Persistence layer
Add Skova.Repository.Impl package to your project(s) of the Persistence layer. This package contains classes that are ready to use:

- **`UnitOfWork<TDbContext>`** - default implementation of IUnitOfWork. Requires to specify db context type as `TDbContext`.
In your Persistence layer, you should implement at least your specifications.

- **`GenericRepository<TDomain, TDb, TDbContext>`** - default generic implementation of the `IRepository` interface. You should specify the type of a domain entity (`TDomain`), type of the corresponding db entity (`TDb`) and it requires specifying db context type (`TDbContext`)

### Types registration in DI
Skova.Repository.DependencyInjection package contains extension methods that make DI registrations of these classes much easier, in fluent style:

```csharp
services.AddUnitOfWorkAsScoped<CompanyDbContext>()
    .AddRepositoryAsScoped<Person, DbPerson>(config => config.AddKeyRecognizer(p => new object[] { p.Id }))
    .AddSpecificationAsTransient<IPersonSpecification, PersonSpecification>();
```

Which is equivalent to:

```csharp
services.AddScoped<IUnitOfWork, UnitOfWork<CompanyDbContext>>();
services.AddScoped<IRepository<Person>, GenericRepository<Person, DbPerson, CompanyDbContext>>();
services.AddTransient<PersonSpecification>();
services.AddTransient<ISpecification<Person>, PersonSpecification>();
services.AddTransient<SpecificationFactory<IPersonSpecification>>(
    sp => () => (IPersonSpecification)sp.GetRequiredService(typeof(PersonSpecification)));
```

`AddUnitOfWorkAsScoped` is used to register `UnitOfWork` for a db context. Then you can chain registrations of repositories associated with this unit-of-work through `AddRepositoryAsScoped` method. During repository registration, you should register specifications for `TDomain` type by one of two ways:

- chain one or more `AddSpecification*` methods right after `AddRepositoryAsScoped`
- using `config` argument of the `AddRepositoryAsScoped` method.

**Important!** that you should register `KeyRecognizer<TDomain>` delegate for any domain type used by the registered `GenericRepository<>`. That can be done through `AddKeyRecognizer` method by chaining it after `AddRepositoryAsScoped` or calling it in the scope of the configuration lambda argument of `AddRepositoryAsScoped`.
