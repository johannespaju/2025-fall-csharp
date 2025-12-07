using System.ComponentModel.DataAnnotations;

namespace ConsoleAppDbTest.Domain;

public class Book : BaseEntity
{
    [MaxLength(255)]
    public string Title { get; set; } = default!;


    public int? PersonId { get; set; }
    public Person? Person { get; set; }
    
    public ICollection<BookAuthor>? BookAuthors { get; set; }

    public override string ToString()
    {
        return Id + " " + Title;
    }
}