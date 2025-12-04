using System.ComponentModel.DataAnnotations;

namespace ConsoleAppDbTest.Domain;

public class Author : BaseEntity
{
    [MaxLength(128)]
    public string Name { get; set; } = default!;
    
    public ICollection<BookAuthor>? AuthorBooks { get; set; }
    
}