using Core.Enum;
using Core.Models;

namespace Application.Interfaces.Repositories;

public interface IAccessRepository
{
    Task<Result<AccessControl>> CheckAccess(Guid userId, Guid documentId);
    Task<Result> SetUserPermission(Guid documentId, Guid userId, Permissions permission);
    Task<Result<IEnumerable<User>>> GetReaders(Guid documentId);
    Task<Result<IEnumerable<User>>> GetWriters(Guid documentId);
    Task<Result<IEnumerable<User>>> GetUsersPermissions(Guid documentId);
}