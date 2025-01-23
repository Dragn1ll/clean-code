using Core.Enum;

namespace Application.Interfaces.Services;

public interface IUserService
{
    Task<Result> Register(string userName, string email, string password);
    Task<Result<string>> Login(string email, string password);
    Task<Result<bool>> CheckById(Guid userId);
    Task<Result<Role>> GetRolById(Guid userId);
}