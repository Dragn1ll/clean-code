using Core.Entities;

namespace Application.Interfaces.Auth;

public interface IJwtWorker
{
    string GenerateToken(User user);
}