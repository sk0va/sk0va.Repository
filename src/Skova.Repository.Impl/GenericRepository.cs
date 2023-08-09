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
    private readonly KeyRecognizer<TDomain> keyRecognizer;

    /// <summary>
    /// Create a new instance of <see cref="GenericRepository{TDomain, TDb, TDbContext}"/>
    /// </summary>
    public GenericRepository(
        IMapper mapper,
        TDbContext dbContext, 
        KeyRecognizer<TDomain> keyRecoginzer) 
    {
        keyRecognizer = keyRecoginzer;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    /// <inheritdoc/>
    public async Task AddAsync(TDomain entity, CancellationToken ct = default)
    {
        var dbEntity = _mapper.Map<TDb>(entity);
        await _dbContext.AddAsync(dbEntity, ct);
    }

    /// <inheritdoc/>
    public void Update(TDomain entity)
    {
        TDb dbEntity = FindEntity(entity);
        _mapper.Map(entity, dbEntity);
        DbSet.Update(dbEntity);
    }

    /// <inheritdoc/>
    public void Delete(TDomain entity)
    {
        TDb dbEntity = FindEntity(entity);
        DbSet.Remove(dbEntity);
    }

    private TDb FindEntity(TDomain entity)
    {
        var key = keyRecognizer(entity);
        var dbEntity = DbSet.Find(key) 
            ?? throw new KeyNotFoundException($"Entity with key {key} not found");

        return dbEntity;
    }

    /// <inheritdoc/>
    public IEntityQuery<TDomain> With(ISpecification<TDomain> specification)
    {
        return new QueryExecutor<TDomain, TDb, TDbContext>(_dbContext, _mapper, specification);
    }
}
