using Application.Utils;

namespace Application.Interfaces.Services;

public interface IMinioService
{
    Task<Result> CreateDocument(Guid documentId);
    Task<Result<string>> PullDocument(Guid documentId); 
    Task<Result> PushDocument(Guid documentId, string content);
    Task<Result> DeleteDocument(Guid documentId);
}