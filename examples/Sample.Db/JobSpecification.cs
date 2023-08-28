using Sample.App;
using Sample.Core;
using Skova.Repository.Impl;

namespace Sample.Db;

// An alternative way to make specifications, without using of QueryTransformationsContainer<>. 
// It's merely optimized from a performance perspective (less objects to allocate in the heap) but requires more code to write.
// You probably may find it useful in some very performance-critical cases.
public class JobSpecification : IJobSpecification, IQueryTransformer<DbJob>
{
    Guid? _id = null;
    string _title = null;
    string _description = null;
    DateTimeOffset? _startDate = null;
    DateTimeOffset? _endDate = null;

    bool? _orderByStartDateDescending = null;
    bool? _orderByEndDateDescending = null;

    public void GetById(Guid id)
    {
        _id = id;
    }

    public void ByTitle(string title) => _title = title;
    public void ByDescription(string description) => _description = description;
    public void ByStartDate(DateTimeOffset startDate) => _startDate = startDate;
    public void ByEndDate(DateTimeOffset endDate) => _endDate = endDate;

    public void OrderByStartDate(bool descending = false) => _orderByStartDateDescending = descending;
    public void OrderByEndDate(bool descending = false) => _orderByEndDateDescending = descending;

    public IQueryable<DbJob> Apply(IQueryable<DbJob> query)
    {
        if (_id != null)
            query = query.Where(p => p.Id == _id);
        
        if (_title != null)
            query = query.Where(p => p.Title == _title);
            
        if (_description != null)
            query = query.Where(p => p.Description == _description);
            
        if (_startDate != null)
            query = query.Where(p => p.StartDate == _startDate);
            
        if (_endDate != null)
            query = query.Where(p => p.EndDate == _endDate);
        
        if (_orderByStartDateDescending != null)
            query = _orderByStartDateDescending.Value
                ? query.OrderByDescending(p => p.StartDate) 
                : query.OrderBy(p => p.StartDate);

        if (_orderByEndDateDescending != null)
            query = _orderByEndDateDescending.Value
                ? query.OrderByDescending(p => p.EndDate) 
                : query.OrderBy(p => p.EndDate);

        return query;
    }
}
