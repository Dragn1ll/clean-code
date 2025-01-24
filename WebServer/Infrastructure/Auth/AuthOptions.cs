namespace Infrastructure.Auth;

public class AuthOptions
{
    public string Issuer { get; } = string.Empty;
    public string Audience { get; } = string.Empty;
    public string SecretKey { get; } = string.Empty;
    public int ExpiresHours { get; }
}