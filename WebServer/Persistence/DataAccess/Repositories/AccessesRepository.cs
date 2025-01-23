using Application;
using Application.Interfaces.Repositories;
using Core.Enum;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.DataAccess.Entities;

namespace Persistence.DataAccess.Repositories;

public class AccessesRepository(WebDbContext dbContext) : IAccessRepository
{
    public async Task<Result<AccessControl>> Check(Guid userId, Guid documentId)
    {
        try
        {
            var access = await dbContext.Accesses
                .FirstOrDefaultAsync(a => a.UserId == userId && a.DocumentId == documentId);
            
            return Result<AccessControl>.Success(new AccessControl(access!.UserId, access.DocumentId, 
                (Permissions)access.PermissionId));
        }
        catch (Exception exception)
        {
            return Result<AccessControl>.Failure(exception);
        }
    }

    public async Task<Result> Create(Guid documentId, Guid userId, Permissions permission)
    {
        try
        {
            await dbContext.Accesses.AddAsync(new AccessEntity()
            {
                DocumentId = documentId,
                UserId = userId,
                PermissionId = (int)permission
            });
            await dbContext.SaveChangesAsync();
            
            return Result.Success();
        }
        catch (Exception exception)
        {
            return Result.Failure(exception);
        }
    }

    public async Task<Result> Set(Guid documentId, Permissions newPermission)
    {
        try
        {
            await dbContext.Accesses
                .Where(a => a.DocumentId == documentId)
                .ExecuteUpdateAsync(s => 
                    s.SetProperty(a => a.PermissionId, (int)newPermission));
            
            return Result.Success();
        }
        catch (Exception exception)
        {
            return Result.Failure(exception);
        }
    }

    public async Task<Result<IEnumerable<User>>> Get(Guid documentId)
    {
        try
        {
            var userEntities = await dbContext.Accesses
                .Where(a => a.DocumentId == documentId)
                .Include(a => a.User)
                .ToListAsync();

            var users = new List<User>();
            
            foreach (var userEntity in userEntities)
            {
                users.Add(new User(userEntity.UserId, userEntity.User.Username, 
                    userEntity.User.Email, userEntity.User.PasswordHash, (Role)userEntity.User.RoleId));
            }
            
            return Result<IEnumerable<User>>.Success(users);
        }
        catch (Exception exception)
        {
            return Result<IEnumerable<User>>.Failure(exception);
        }
    }

    public async Task<Result<IEnumerable<User>>> GetReaders(Guid documentId)
    {
        try
        {
            var userEntities = await dbContext.Accesses
                .Where(a => a.DocumentId == documentId && a.PermissionId == (int)Permissions.Read)
                .Include(a => a.User)
                .ToListAsync();

            var users = new List<User>();
            
            foreach (var userEntity in userEntities)
            {
                users.Add(new User(userEntity.UserId, userEntity.User.Username, 
                    userEntity.User.Email, userEntity.User.PasswordHash, (Role)userEntity.User.RoleId));
            }
            
            return Result<IEnumerable<User>>.Success(users);
        }
        catch (Exception exception)
        {
            return Result<IEnumerable<User>>.Failure(exception);
        }
    }

    public async Task<Result<IEnumerable<User>>> GetWriters(Guid documentId)
    {
        try
        {
            var userEntities = await dbContext.Accesses
                .Where(a => a.DocumentId == documentId && a.PermissionId == (int)Permissions.Write)
                .Include(a => a.User)
                .ToListAsync();

            var users = new List<User>();
            
            foreach (var userEntity in userEntities)
            {
                users.Add(new User(userEntity.UserId, userEntity.User.Username, 
                    userEntity.User.Email, userEntity.User.PasswordHash, (Role)userEntity.User.RoleId));
            }
            
            return Result<IEnumerable<User>>.Success(users);
        }
        catch (Exception exception)
        {
            return Result<IEnumerable<User>>.Failure(exception);
        }
    }
}