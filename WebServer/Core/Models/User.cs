using Core.Enum;

namespace Core.Models;

public class User(Guid id, string name, string email, string password, Role role = Role.User)
{
    public Guid Id { get; } = id;
    public string Name { get; } = name;
    public string Email { get; } = email;
    public string Password { get; } = password;
    public Role Role { get; } = role;
}