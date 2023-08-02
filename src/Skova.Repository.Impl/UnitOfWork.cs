using Microsoft.EntityFrameworkCore;
using Skova.Repository.Abstractions;

namespace Skova.Repository.Impl;

/// <summary>
/// Generic implementation of <see cref="IUnitOfWork"/>
/// </summary>
public class UnitOfWork<TDbContext> : IUnitOfWork
    where TDbContext : DbContext
{
    private readonly TDbContext _context;

    /// <summary>
    /// Create a new instance of <see cref="UnitOfWork{TDbContext}"/>
    /// </summary>
    public UnitOfWork(TDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _context.SaveChangesAsync(ct);
    }
}
