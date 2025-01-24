using Application.Utilis;
using Core.Enum;
using Core.Models;

namespace Application.Interfaces.Repositories;

public interface IUsersRepository
{
    Task<Result<bool>> CheckById(Guid userId);
    Task<Result> Create(User user);
    Task<Result<User>> GetByEmail(string email);
    Task<Result<User>> GetById(Guid userId);
    Task<Result<Role>> GetRoleById(Guid userId);
}