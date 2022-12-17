using dominikz.shared.Contracts;

namespace dominikz.api.Models;

public class Person
{
    public Guid Id { get; set; }
    public Guid? FileId { get; set; }
    public string Name { get; set; }
    public PersonCategoryFlags Category { get; set; }

    public List<Article> Articles { get; set; } = new();
    public StorageFile? File { get; set; }

    public Person()
    {
        Name = string.Empty;
    }
}
