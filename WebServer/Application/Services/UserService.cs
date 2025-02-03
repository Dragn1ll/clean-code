using Application.Interfaces.Auth;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Utils;
using Core.Enum;
using Core.Models;

namespace Application.Services;

public class UserService(IUsersRepository usersRepository, IJwtWorker jwtWorker)
    : IUserService
{
    public async Task<Result> CheckById(Guid userId)
    {
        var checkResult = await usersRepository.CheckById(userId);
        
        if (!checkResult.IsSuccess)
            return Result.Failure(checkResult.Error);
        
        if (!checkResult.Value)
            return Result.Failure(new Error(ErrorType.NotFound, 
                "Такого пользователя не существует!"));
        
        return Result.Success();
    }
    
    public async Task<Result> CheckByEmail(string email)
    {
        var checkResult = await usersRepository.CheckByEmail(email);
        
        if (!checkResult.IsSuccess)
            return Result.Failure(checkResult.Error);
        
        if (!checkResult.Value)
            return Result.Failure(new Error(ErrorType.NotFound, 
                "Такого пользователя не существует!"));
        
        return Result.Success();
    }
    
    public async Task<Result> Register(string userName, string email, string password)
    {
        var checkEmail = await CheckByEmail(email);
        if (checkEmail is { IsSuccess: true })
            return Result.Failure(new Error(ErrorType.BadRequest, 
                "Пользователь с таким email уже существует!"));
        
        var user = User.Create(new Guid(), userName, email, password);
        
        return await usersRepository.Create(user);
    }

    public async Task<Result<string>> Login(User user)
    {
        var token = jwtWorker.GenerateToken(user);
        return Result<string>.Success(token);
    }

    public async Task<Result<User>> GetById(Guid userId)
    {
        var checkIdResult = await usersRepository.GetById(userId);
        if (!checkIdResult.IsSuccess)
            return Result<User>.Failure(checkIdResult.Error);
        
        return await usersRepository.GetById(userId);
    }

    public async Task<Result<User>> GetByEmail(string email)
    {
        var checkEmailResult = await CheckByEmail(email);
        if (!checkEmailResult.IsSuccess)
            return Result<User>.Failure(checkEmailResult.Error);
        
        return await usersRepository.GetByEmail(email);
    }

    public async Task<Result<Role>> GetRolById(Guid userId)
    {
        var checkIdResult = await usersRepository.GetById(userId);
        if (!checkIdResult.IsSuccess)
            return Result<Role>.Failure(checkIdResult.Error);
        
        return await usersRepository.GetRoleById(userId);
    }
}