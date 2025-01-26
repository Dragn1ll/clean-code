namespace Persistence.DataAccess.Entities;

public class AccessEntity
{
    public Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;
    
    public int PermissionId { get; set; }
    public PermissionEntity Permission { get; set; } = null!;
    
    public Guid DocumentId { get; set; }
    public DocumentEntity Document { get; set; } = null!;
}