using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Skova.Repository.Abstractions;

namespace Skova.Repository.Impl;

/// <summary>
/// Default implementation of <see cref="IEntityUpdater{TDomain}"/>
/// <typeparamref name="TDomain">Domain type of entities to query. Implementations of this type should care about mapping between domain layer and underlying data layer<typeparamref>
/// </summary>
public class EntityUpdater<TDomain, TDb> : IEntityUpdater<TDomain>
{
    private static readonly ParameterExpression _target = Expression.Parameter(typeof(SetPropertyCalls<TDb>), "setter");
    private Expression _setCallsChain = _target;

    protected IMapper Mapper { get; }

    public EntityUpdater(IMapper mapper)
    {
        Mapper = mapper;
    }

    /// <inheritdoc/>
    public IEntityUpdater<TDomain> Set<TValue>(Expression<Func<TDomain, TValue>> propertySetter, TValue value)
    {
        var propertyTypeArg = Type.MakeGenericMethodParameter(0);

        var miSetProperty = typeof(SetPropertyCalls<TDb>)
            .GetMethod(
                nameof(SetPropertyCalls<TDb>.SetProperty),
                1,
                new[]
                {
                    typeof(Func<,>).MakeGenericType(typeof(TDb), propertyTypeArg),
                    propertyTypeArg
                });

        var setExpression = Mapper.Map<Expression<Func<TDb, TValue>>>(propertySetter);

        _setCallsChain = Expression.Call(
            _setCallsChain,
            miSetProperty.MakeGenericMethod(typeof(TValue)),
            new Expression[] { setExpression, Expression.Constant(value, typeof(TValue)) });

        return this;
    }

    /// <summary>
    /// Generates expression that represents update operation for db entity
    /// </summary>
    public Expression<Func<SetPropertyCalls<TDb>, SetPropertyCalls<TDb>>> GenerateUpdateExpression()
    {
        var lambda = Expression.Lambda<Func<SetPropertyCalls<TDb>, SetPropertyCalls<TDb>>>(_setCallsChain, false, new[] { _target });

        return lambda;
    }
}
