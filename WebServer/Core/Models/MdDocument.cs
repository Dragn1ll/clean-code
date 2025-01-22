namespace Core.Models;

public class MdDocument(Guid id, Guid masterId, string title, DateTime creationDate)
{
    public Guid Id { get; } = id;
    public Guid MasterId { get; } = masterId;
    public string Title { get; } = title;
    public DateTime CreationDate { get; } = creationDate;
}