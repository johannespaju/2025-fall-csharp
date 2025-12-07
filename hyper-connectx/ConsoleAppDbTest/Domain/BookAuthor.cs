namespace ConsoleAppDbTest.Domain;

public class BookAuthor : BaseEntity
{
    // fk is mandatory
    public int BookId { get; set; }
    // entity is nullable because maybe didnt do sql join
    public Book? Book { get; set; }
    
    public int AuthorId { get; set; }
    public Author? Author { get; set; }
    
    public override string ToString()
    {
        return Id + " BookId:" + BookId + " AuthorId:" + AuthorId;
    }
}