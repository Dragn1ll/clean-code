using Core.Entities;

namespace Application.Interfaces;

public interface IDocumentService
{
    Task<Result> Create(long userId, string title);
    Task<Result> Delete(long documentId);
    Task<Result> Rename(long documentId, string newTitle);
    Task<Result<MdDocument>> Get(long documentId);
    Task<Result<IEnumerable<MdDocument>>> GetUserDocuments(long userId);
    Task<Result<IEnumerable<MdDocument>>> GetUserPermission(long userId);
}