namespace Sample.App;

public class DbPerson : DbEntity
{
    public string Name { get; set; }
    public int Age { get; set; }

    public ICollection<DbJob> Jobs { get; set; }
}
