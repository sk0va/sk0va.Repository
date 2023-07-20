using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Skova.Repository.Abstractions;
using Skova.Repository.Abstractions.Specifications;

namespace Skova.Repository.Impl;

public class GenericRepository<TDomain, TDb, TDbContext> : IRepository<TDomain>
    where TDbContext : DbContext
    where TDb : class
{
    private readonly TDbContext _dbContext;

    private DbSet<TDb> DbSet => _dbContext.Set<TDb>();

    private readonly IMapper _mapper;

    public GenericRepository(
        IMapper mapper,
        TDbContext dbContext)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<TDomain> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var dbEntity = await _dbContext.FindAsync<TDb>(id, ct);
        return _mapper.Map<TDb, TDomain>(dbEntity);
    }

    public async Task AddAsync(TDomain entity, CancellationToken ct)
    {
        var dbEntity = _mapper.Map<TDb>(entity);
        await _dbContext.AddAsync(dbEntity, ct);
    }

    public void Update(TDomain entity)
    {
        var dbEntity = _mapper.Map<TDb>(entity);
        DbSet.Update(dbEntity);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        TDb db = await _dbContext.FindAsync<TDb>(id, ct);
        DbSet.Remove(db);
    }

    public IEntitySet<TDomain> With(ISpecification<TDomain> specification)
    {
        return new EntitySet<TDomain, TDb, TDbContext>(_dbContext, _mapper, specification);
    }
}
