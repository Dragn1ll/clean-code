using Application.Utils;
using Core.Enum;
using Core.Models;

namespace Application.Interfaces.Repositories;

public interface IAccessRepository
{
    Task<Result<AccessControl>> Check(Guid userId, Guid documentId);
    Task<Result> Create(Guid documentId, Guid userId, Permissions permission);
    Task<Result> Set(Guid documentId, Permissions newPermission);
    Task<Result<IEnumerable<User>>> Get(Guid documentId);
    Task<Result<IEnumerable<User>>> GetReaders(Guid documentId);
    Task<Result<IEnumerable<User>>> GetWriters(Guid documentId);
}