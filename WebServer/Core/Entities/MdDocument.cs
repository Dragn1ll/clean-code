namespace Core.Entities;

public class MdDocument(long id, long masterId, string title, DateTime creationDate)
{
    public long Id { get; } = id;
    public long MasterId { get; } = masterId;
    public string Title { get; } = title;
    public DateTime CreationDate { get; } = creationDate;
}