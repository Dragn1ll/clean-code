namespace Infrastructure.DataAccess.Entities;

public class RoleEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public IEnumerable<UserEntity> Users { get; set; } = new List<UserEntity>();
}