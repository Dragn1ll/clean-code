namespace Persistence.DataAccess.Entities;

public class UserEntity
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public RoleEntity Role { get; set; } = null!;
    public ICollection<DocumentEntity> Documents { get; set; } = new List<DocumentEntity>();
    public ICollection<AccessEntity> Accesses { get; set; } = new List<AccessEntity>();
}