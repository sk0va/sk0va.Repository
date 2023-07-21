namespace Skova.Repository.Impl;

public interface IQueryTransformer<T>
{
    IQueryable<T> Apply(IQueryable<T> query);
}
