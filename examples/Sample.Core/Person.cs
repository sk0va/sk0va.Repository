namespace Sample.Core;

public class Person : Entity
{
    public string PersonName { get; set; }
    public int PersonAge { get; set; }

    public ICollection<Job> Jobs { get; set; }
}
