using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.DataAccess.Entities;

public class DocumentEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid AuthorId { get; set; }
    public UserEntity Author { get; set; } = null!;
    public ICollection<UsersPermissionsEntity> Users { get; set; } = new List<UsersPermissionsEntity>();
}