namespace Skova.Repository.Impl;

/// <summary> 
/// Transforms a query to data storage by applying query rules defined in specification
/// <typeparamref name="TDb">Db entity type</typeparamref>
/// </summary>
public interface IQueryTransformer<TDb>
{
    /// <summary>
    /// Apply query rules defined in specification to provided query
    /// <param name="query">Query to apply rules to</param>
    /// </summary>
    IQueryable<TDb> Apply(IQueryable<TDb> query);
}
