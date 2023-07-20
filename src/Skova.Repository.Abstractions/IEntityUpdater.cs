using System.Linq.Expressions;

namespace Skova.Repository.Abstractions;

public interface IEntityUpdater<T>
{
    IEntityUpdater<T> Set<TValue>(Expression<Func<T, TValue>> property, TValue value);
}
