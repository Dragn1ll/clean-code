namespace Persistence.DataAccess.Entities;

public class PermissionEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<AccessEntity> UsersDocuments { get; set; } = new List<AccessEntity>();
}