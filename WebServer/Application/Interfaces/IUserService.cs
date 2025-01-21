using Core.Entities;
using Core.Enum;

namespace Application.Interfaces;

public interface IUserService
{
    Task<Result<bool>> CheckById(long userId);
    Task<Result<User>> GetUserByEmail(string email);
    Task<Result<Role>> GetUserRoleById(long userId);
    Task<Result> Register(string userName, string email, string password);
    Task<Result<User>> Login(string email, string password);
}