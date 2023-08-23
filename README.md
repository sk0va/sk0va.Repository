[![codecov](https://codecov.io/gh/sk0va/sk0va.Repository/graph/badge.svg?token=TDHXGUH5NZ)](https://codecov.io/gh/sk0va/sk0va.Repository) [![NuGet](http://img.shields.io/nuget/vpre/Skova.Repository.Abstractions.svg?label=NuGet)](https://www.nuget.org/packages/Skova.Repository.Abstractions/)

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

- **`IUnitOfWork`** - represents unit-of-work that will be used to manage entities. UnitOfWork is usually registered in DI containers as scoped associated with some business transaction. Skova.Repository.Imp
Where and how to leave feedback such as link to the project issues, Twitter, bug trry for a domain layer's entity of the specified `TDomain` type. Supports operations for creating updating and deleting entities from a unit of work using `AddAsync`, `Update` and `Delete` methods. To load entities from storage to unit-of-work or get them from storage without attaching them to the unit-of-work, you should call method `With` and provide a specification of the entities you want to receive. Also, it supports the updation and deletion of entities that fit provided specification directly in storage like EF Core do (See "[ExecuteUpdate and ExecuteDelete](https://learn.microsoft.com/en-us/ef/core/saving/execute-insert-update-delete)" article in the MSDN documentation for details). You should use the method `With` first and provide specifications that will be used to define entities you want to update or delete.

- **`ISpecification<TDomain>`** - represents a specification for a domain layer's entity of the specified `TDomain` type. Specifications are used to customize queries when you want to load data from storage. This library doesn't strict developers to use some API for specifications and leaves it up to you how to design specifications API.

Sample specification may look like this:
```csharp
public interface IPersonSpecification : ISpecification<Person>
{
    public void GetById(Guid id);
    public void ByName(string name, bool exactMatch = false);
    public void MinimalAge(int age);

    public void OrderByAge(bool descending = false);
}
```

Also, you may organize some of specifications into a hierarchy:

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

- **`QueryTransformationsContainer<TDb>`** - tool for containing query transformations. This class is useful in specification implementation classes (see the following examples). De facto `QueryTransformationsContainer<>` just wraps `List<Func<IQueryable<TDb>, IQueryable<TDb>>>` and implements `IQueryTranformation<>`.

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

`AddUnitOfWorkAsScoped` is used to register `UnitOfWork` for a db context. Then you can chain registrations of repositories associated with this unit-of-work through `AddRepositoryAsScoped` method. During repository registration, you should register specifications for `TDomain` type in one of two ways:

- chain one or more `AddSpecification*` methods right after `AddRepositoryAsScoped`
- using `config` argument of the `AddRepositoryAsScoped` method.

**Important!** that you should register `KeyRecognizer<TDomain>` delegate for any domain type used by the registered `GenericRepository<>`. That can be done through `AddKeyRecognizer` method by chaining it after `AddRepositoryAsScoped` or calling it in the scope of the configuration lambda argument of `AddRepositoryAsScoped`.

### Implementing specifications

Typical specification implementation is working in two phases:

1. Specify criteria needed to determine a set of domain entities for further processing (like retrieving from storage, updation or deletion)
2. Apply transformation for `IQuery<TDb>` (DbSet usually) which will be used by EF Core for further processing of target db entities

Inside your specification use `QueryTransformationsContainer<>` to accumulate query transformations in phase (1) and apply transformations in phase (2)
Your specification implementation class should

- Implement `ISpecification<TDomain>` interface for target domain entity of `TDomain` type, which will be used in phase (1)
- Implement `IQueryTransformer<TDb>` to apply transformation in phase (2)

Example:
```csharp
public class EntitySpecification : QueryTransformationsContainer<TDb>, IEntitySpecification
{
    public void GetById(Guid id)
    {
        AddTranformation(q => q.Where(p => p.Id == id));
    }
}
```

In the case of hierarchies of entities, specification implementation became more tricky. We collect query transformations in `QueryTransformationsContainer<TDb>` But we can't just re-use `QueryTransformationsContainer<DbEntity>` in children's classes, because we can't pass transformations for inherited db entities like `_container.AddTranformation((IQueryable<DbPerson> q) => q.Where(Id = id))` - compilator cannot convert that lambda expression to `Func<IQueryable<DbEntity>, IQueryable<DbEntity>>`. But you may do this in this way:

```csharp
public class EntitySpecification<TDomain, TDb> : QueryTransformationsContainer<TDb>, IEntitySpecification
    where TDomain : Entity
    where TDb : DbEntity
{
    public void GetById(Guid id)
    {
        AddTranformation(q => q.Where(p => p.Id == id));
    }
}

public class PersonSpecification : EntitySpecification<Person, DbPerson>, IPersonSpecification
{
    public void ByName(string name, bool exactMatch = false)
    {
        AddTranformation(q => q.Where(p => p.Name == name));
    }

    public void MinimalAge(int age)
    {
        AddTranformation(q => q.Where(p => p.Age >= age));
    }

    public void OrderByAge(bool descending = false)
    {
        AddTranformation(q => descending ? q.OrderByDescending(p => p.Age) : q.OrderBy(p => p.Age));
    }
}
```

Here `EntitySpecification` is a base class for specifications as well as Entity is the root base class for entities like Person. Note, that we use parametrization with constraint for type parameters to Entities-derived classes for both layers domain and db. This is required to make all `IQuery<TDb>` transformations be typed with the most specific entity type.

When you need to add other specification classes which inherit `PersonSpecification`, you should change the `PersonSpecification` signature to make it generic as follow:

```csharp
public class PersonSpecification<TPerson, TDbPerson> : EntitySpecification<TPerson, TDbPerson>, IPersonSpecification
    where TPerson : Person    
    where TDbPerson : DbPerson
```

So inherited from `PersonSpecification<>` classes will specify specific domain and db entity types they need. I.e.:

```csharp
public class EmployeeSpecification : PersonSpecification<TEmployee, TDbEmployee>, IEmployeeSpecification
```
