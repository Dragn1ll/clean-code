using Application.Utils;
using Core.Enum;
using Core.Models;

namespace Application.Interfaces.Services;

public interface IAccessService
{
    Task<Result<AccessControl>> Check(Guid userId, Guid documentId);
    Task<Result> CheckMaster(Guid userId, Guid documentId);
    Task<Result> Create(Guid userId, Guid documentId, Permissions permission);
    Task<Result> Set(Guid userId, Guid documentId, Permissions newPermission);
    Task<Result<IEnumerable<User>>> GetUsers(Guid documentId);
    Task<Result<IEnumerable<User>>> GetReaders(Guid documentId);
    Task<Result<IEnumerable<User>>> GetWriters(Guid documentId);
    Task<Result> Delete(Guid documentId, Guid userId);
}