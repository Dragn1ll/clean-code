namespace Persistence.DataAccess.Entities;

public class DocumentEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid AuthorId { get; set; }
    public UserEntity Author { get; set; } = null!;
    public ICollection<AccessEntity> Accesses { get; set; } = new List<AccessEntity>();
}