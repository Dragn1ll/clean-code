using Application.Utils;
using Core.Models;

namespace Application.Interfaces.Repositories;

public interface IDocumentsRepository
{
    Task<Result> Create(MdDocument mdDocument);
    Task<Result> Delete(Guid documentId);
    Task<Result> Rename(Guid documentId, string newTitle);
    Task<Result<MdDocument>> GetById(Guid documentId);
    Task<Result<bool>> Check(Guid documentId);
    Task<Result<IEnumerable<MdDocument>>> GetByUser(Guid userId);
    Task<Result<IEnumerable<MdDocument>>> GetByUserPermission(Guid userId);
}