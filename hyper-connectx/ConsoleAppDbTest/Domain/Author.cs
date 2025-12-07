using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ConsoleAppDbTest.Domain;

[Index(nameof(Name), IsUnique =  true)]
public class Author : BaseEntity
{
    [MaxLength(128)]
    public string Name { get; set; } = default!;
    
    public ICollection<BookAuthor>? AuthorBooks { get; set; }
    
    public override string ToString()
    {
        return Id + " " + Name;
    }
}