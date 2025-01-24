using Application;
using Application.Interfaces.Auth;
using Application.Interfaces.Repositories;
using Application.Utilis;
using Core.Enum;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.DataAccess.Entities;

namespace Persistence.DataAccess.Repositories;

public class UsersRepository(WebDbContext dbContext, IPasswordHasher passwordHasher) : IUsersRepository
{
    public async Task<Result<bool>> CheckById(Guid userId)
    {try
        {
            var user = await dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);
        
            return Result<bool>.Success(user is not null);
        }
        catch (Exception exception)
        {
            return Result<bool>.Failure(exception);
        }
    }

    public async Task<Result> Create(User user)
    {
        try
        {
            await dbContext.Users.AddAsync(new UserEntity
            {
                Id = Guid.NewGuid(),
                Username = user.Name,
                Email = user.Email,
                PasswordHash = passwordHasher.Hash(user.Password),
                RoleId = (int)user.Role
            });
            await dbContext.SaveChangesAsync();
        
            return Result.Success();
        }
        catch (Exception exception)
        {
            return Result.Failure(exception);
        }
    }

    public async Task<Result<User>> GetByEmail(string email)
    {
        try
        {
            var userEntity = await dbContext.Users
                                .AsNoTracking()
                                .FirstOrDefaultAsync(u => u.Email == email);
            
            return Result<User>.Success(new User(userEntity!.Id, userEntity.Username, 
                userEntity.Email, userEntity.PasswordHash, (Role)userEntity.RoleId));
        }
        catch (Exception exception)
        {
            return Result<User>.Failure(exception);
        }
    }

    public async Task<Result<User>> GetById(Guid userId)
    {
        try
        {
            var userEntity = await dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);
            
            return Result<User>.Success(new User(userEntity!.Id, userEntity.Username, 
                userEntity.Email, userEntity.PasswordHash, (Role)userEntity.RoleId));
        }
        catch (Exception exception)
        {
            return Result<User>.Failure(exception);
        }
    }

    public async Task<Result<Role>> GetRoleById(Guid userId)
    {
        try
        {
            var userEntity = await dbContext.Users
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(u => u.Id == userId);
            
            return Result<Role>.Success((Role)userEntity!.RoleId);
        }
        catch (Exception exception)
        {
            return Result<Role>.Failure(exception);
        }
    }
}