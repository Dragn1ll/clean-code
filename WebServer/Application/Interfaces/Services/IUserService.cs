using Application.Utils;
using Core.Enum;
using Core.Models;

namespace Application.Interfaces.Services;

public interface IUserService
{
    Task<Result> CheckById(Guid userId);
    Task<Result> CheckByEmail(string email);
    Task<Result> Register(string userName, string email, string password);
    Task<Result<string>> Login(string email, string password);
    Task<Result<User>> GetById(Guid userId);
    Task<Result<User>> GetByEmail(string email);
    Task<Result<Role>> GetRolById(Guid userId);
}