namespace Core.Models;

public class MdDocument(Guid id, Guid authorId, string title, DateTime creationDate)
{
    public Guid Id { get; } = id;
    public Guid AuthorId { get; } = authorId;
    public string Title { get; } = title;
    public DateTime CreationDate { get; } = creationDate;
}