using Application.Interfaces.Auth;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Utils;
using Core.Enum;
using Core.Models;

namespace Application.Services;

public class UserService(IUsersRepository usersRepository, IPasswordHasher passwordHasher, IJwtWorker jwtWorker)
    : IUserService
{
    public async Task<Result> Register(string userName, string email, string password)
    {
        var checkEmail = await usersRepository.GetByEmail(email);
        if (checkEmail is { IsSuccess: true, Value: not null })
            return Result.Failure(new Error(ErrorType.BadRequest, 
                "Пользователь с таким email уже существует!"));
        
        var user = new User(Guid.NewGuid(), userName, email, password);
        
        return await usersRepository.Create(user);
    }

    public async Task<Result<string>> Login(string email, string password)
    {
        var getResult = await usersRepository.GetByEmail(email); 
        if (!getResult.IsSuccess)
            return Result<string>.Failure(new Error(ErrorType.BadRequest, 
                "Не существует аккаунта с таким email!"));
        
        var user = getResult.Value;
        var passwordIsValid = passwordHasher.Validate(password, user!.Password);

        if (!passwordIsValid)
            return Result<string>.Failure(new Error(ErrorType.BadRequest, "Неправильный пароль!"));

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