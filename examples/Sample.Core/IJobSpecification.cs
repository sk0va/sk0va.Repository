using Skova.Repository.Abstractions.Specifications;

namespace Sample.Core;

public interface IJobSpecification : ISpecification<Job>, IEntitySpecification
{
    void ByTitle(string title);
    void ByDescription(string description);
    void ByStartDate(DateTimeOffset startDate);
    void ByEndDate(DateTimeOffset endDate);

    void OrderByStartDate(bool descending = false);
    void OrderByEndDate(bool descending = false);
}