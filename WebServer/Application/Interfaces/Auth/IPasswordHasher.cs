namespace Application.Interfaces.Auth;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Validate(string passwordHash, string password);
}