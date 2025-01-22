using Core.Entities;

namespace Application.Interfaces.Services;

public interface IDocumentService
{
    Task<Result> Create(Guid userId, string title);
    Task<Result> Delete(Guid documentId);
    Task<Result> Rename(Guid documentId, string newTitle);
    Task<Result<MdDocument>> Get(Guid documentId);
    Task<Result<IEnumerable<MdDocument>>> GetUserDocuments(Guid userId);
    Task<Result<IEnumerable<MdDocument>>> GetUserPermission(Guid userId);
}