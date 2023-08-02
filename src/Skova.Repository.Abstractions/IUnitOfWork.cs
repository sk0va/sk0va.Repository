namespace Skova.Repository.Abstractions;

/// <summary>
/// Represents Unit of work pattern
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Save changes to the database
    /// </summary>
    Task SaveChangesAsync(CancellationToken ct);
}
