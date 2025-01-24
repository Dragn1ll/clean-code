using System.Security.Cryptography;
using Application.Interfaces.Auth;
using BCrypt.Net;

namespace Infrastructure.Auth;

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password) => BCrypt.Net.BCrypt.EnhancedHashPassword(password);
    
    public bool Validate(string passwordHash, string password) => 
        BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash);
}