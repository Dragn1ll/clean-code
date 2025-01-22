namespace Infrastructure.DataAccess.Entities;

public class UsersPermissionsEntity
{
    public Guid UserId { get; set; }
    public UserEntity User { get; set; } = new UserEntity();
    public int PermissionId { get; set; }
    public PermissionEntity Permission { get; set; } = new PermissionEntity();
    public Guid DocumentId { get; set; }
    public DocumentEntity Document { get; set; } = new DocumentEntity();
}