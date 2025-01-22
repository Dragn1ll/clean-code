using System.ComponentModel.DataAnnotations;

namespace Infrastructure.DataAccess.Entities;

public class UserEntity
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public Guid RoleId { get; set; }
    public RoleEntity Role { get; set; } = null!;
    public ICollection<DocumentEntity> AuthoredDocuments { get; set; } = new List<DocumentEntity>();
    public ICollection<UsersPermissionsEntity> AssignedPermissions { get; set; } = new List<UsersPermissionsEntity>();
}