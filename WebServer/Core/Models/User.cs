using Core.Enum;

namespace Core.Models;

public class User
{
    public Guid Id { get; }
    public string Name { get; }
    public string Email { get; }
    public string Password { get; }
    public Role Role { get; }
    
    private User(Guid id, string name, string email, string password, Role role = Role.User)
    {
        Id = id;
        Name = name;
        Email = email;
        Password = password;
        Role = role;
    }

    public static User Create(Guid userId, string name, string email, string password, Role role = Role.User)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Name cannot be null or empty.", nameof(name));

        if (string.IsNullOrEmpty(email))
            throw new ArgumentException("Email cannot be null or empty.", nameof(email));

        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password cannot be null or empty.", nameof(password));

        return new User(userId, name, email, password, role);
    }
}