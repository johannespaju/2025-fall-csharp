using System.ComponentModel.DataAnnotations;

namespace ConsoleAppDbTest.Domain;

public class Person : BaseEntity
{
    [MaxLength(128)]
    public string FirstName { get; set; } = default!;
    
    [MaxLength(128)]
    public string LastName { get; set; } = default!;
    
    public ICollection<Book>? Books { get; set; }

    public override string ToString()
    {
        return $"{Id} {FirstName} {LastName} Books count:{Books?.Count.ToString() ?? "null"}";
    }
}