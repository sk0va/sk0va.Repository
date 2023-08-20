using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Skova.Repository.Abstractions.Specifications;
using Skova.Repository.Abstractions;

namespace Skova.Repository.Impl;

/// <summary>
/// Default implementation of <see cref="IEntityQuery{TDomain}"/>
/// </summary>
public class QueryExecutor<TDomain, TDb, TDbContext>
 : IEntityQuery<TDomain>
    where TDbContext : DbContext
    where TDb : class
{
    private IQueryTransformer<TDb> DbQueryTransformer => (IQueryTransformer<TDb>)Specification;

    protected TDbContext DbContext { get; set; }
    protected IMapper Mapper { get; set; }
    protected ISpecification<TDomain> Specification { get; set; }

    public QueryExecutor(TDbContext dbContext, IMapper mapper, ISpecification<TDomain> specification)
    {
        DbContext = dbContext;
        Mapper = mapper;
        Specification = specification;
    }

    /// <inheritdoc/>
    public async Task<IList<TDomain>> ExecuteQueryAsync(CancellationToken ct = default)
    {
        var q = NewQuery();
        return Mapper.Map<List<TDomain>>(await q.ToListAsync(ct));
    }

    /// <inheritdoc/>
    public async Task<IList<TDomain>> LoadAsync(CancellationToken ct = default)
    {
        var q = NewQuery(false);
        return Mapper.Map<List<TDomain>>(await q.ToListAsync(ct));
    }
    
    /// <inheritdoc/>
    public async Task ExecuteDeleteAllAsync(CancellationToken ct = default)
    {
        var q = NewQuery();
        await q.ExecuteDeleteAsync(ct);
    }

    /// <inheritdoc/>
    public async Task ExecuteUpdateAsync(
        Func<IEntityUpdater<TDomain>, IEntityUpdater<TDomain>> configureEntityUpdater,
        CancellationToken ct = default)
    {
        var q = NewQuery();

        IEntityUpdater<TDomain> updater = new EntityUpdater<TDomain, TDb>(Mapper);
        updater = configureEntityUpdater(updater);

        var updateExpression = ((EntityUpdater<TDomain, TDb>)updater).GenerateUpdateExpression();

        await q.ExecuteUpdateAsync(updateExpression, ct);
    }

    private IQueryable<TDb> NewQuery(bool AsNoTracking = true)
    {
        IQueryable<TDb> q = DbContext.Set<TDb>();
        
        if (AsNoTracking)
            q = q.AsNoTracking();

        return DbQueryTransformer.Apply(q);
    }
}
