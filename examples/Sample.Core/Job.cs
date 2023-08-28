namespace Sample.Core;

public class Job : Entity
{
    public string JobTitle { get; set; }
    public string JobDescription { get; set; }

    public DateTimeOffset JobStartDate { get; set; }
    public DateTimeOffset? JobEndDate { get; set; }
}