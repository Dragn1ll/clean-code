using Core.Models;

namespace Application.Interfaces.Services;

public interface IUserService
{
    Task<Result> Register(string userName, string email, string password);
    Task<Result<User>> Login(string email, string password);
}