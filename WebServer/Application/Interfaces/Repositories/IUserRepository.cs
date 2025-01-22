using Application.Dto;
using Core.Entities;
using Core.Enum;

namespace Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<Result<bool>> CheckById(Guid userId);
    Task<Result> CreateUser(User user);
    Task<Result<UserDto>> GetUserByEmail(string email);
    Task<Result<Role>> GetUserRoleById(Guid userId);
}