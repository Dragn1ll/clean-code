namespace Infrastructure.DataAccess.Entities;

public class PermissionEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<UsersPermissionsEntity> UsersDocuments { get; set; } = new List<UsersPermissionsEntity>();
}