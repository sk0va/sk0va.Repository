using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Skova.Repository.Abstractions;
using Skova.Repository.Abstractions.Specifications;

namespace Skova.Repository.Impl;

/// <summary>
/// Generic implementation of <see cref="IRepository{TDomain}"/>
/// </summary>
public class GenericRepository<TDomain, TDb, TDbContext> : IRepository<TDomain>
    where TDbContext : DbContext
    where TDb : class
{
    private readonly TDbContext _dbContext;

    private DbSet<TDb> DbSet => _dbContext.Set<TDb>();

    private readonly IMapper _mapper;

    /// <summary>
    /// Create a new instance of <see cref="GenericRepository{TDomain, TDb, TDbContext}"/>
    /// </summary>
    public GenericRepository(
        IMapper mapper,
        TDbContext dbContext)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    /// <inheritdoc/>
    public async Task AddAsync(TDomain entity, CancellationToken ct)
    {
        var dbEntity = _mapper.Map<TDb>(entity);
        await _dbContext.AddAsync(dbEntity, ct);
    }

    /// <inheritdoc/>
    public void Update(TDomain entity)
    {
        var dbEntity = _mapper.Map<TDb>(entity);
        DbSet.Update(dbEntity);
    }

    /// <inheritdoc/>
    public void Delete(TDomain entity)
    {
        var dbEntity = _mapper.Map<TDb>(entity);
        DbSet.Remove(dbEntity);
    }

    /// <inheritdoc/>
    public IEntityQuery<TDomain> With(ISpecification<TDomain> specification)
    {
        return new EntityQuery<TDomain, TDb, TDbContext>(_dbContext, _mapper, specification);
    }
}
