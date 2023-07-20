namespace Skova.Repository.Impl;

internal interface IQueryTransformer<T>
{
    IQueryable<T> Apply(IQueryable<T> query);
}
