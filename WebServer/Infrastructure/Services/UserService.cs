using Application;
using Application.Interfaces.Auth;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Core.Enum;
using Core.Models;

namespace Infrastructure.Services;

public class UserService(IUsersRepository usersRepository, IPasswordHasher passwordHasher, IJwtWorker jwtWorker)
    : IUserService
{
    public async Task<Result> Register(string userName, string email, string password)
    {
        var passwordHash = passwordHasher.Hash(password);
        var user = new User(Guid.NewGuid(), userName, email, passwordHash);
        
        return await usersRepository.Create(user);
    }

    public async Task<Result<string>> Login(string email, string password)
    {
        var getResult = await usersRepository.GetByEmail(email); 
        if (!getResult.IsSuccess)
            return Result<string>.Failure(getResult.Error);
        
        var user = getResult.Value;
        var passwordIsValid = passwordHasher.Validate(user!.Password, password);

        if (!passwordIsValid)
            return Result<string>.Failure(new ArgumentException("Invalid password"));

        var token = jwtWorker.GenerateToken(user);
        return Result<string>.Success(token);
    }

    public async Task<Result<bool>> CheckById(Guid userId)
    {
        return await usersRepository.CheckById(userId);
    }

    public async Task<Result<Role>> GetRolById(Guid userId)
    {
        return await usersRepository.GetRoleById(userId);
    }
}