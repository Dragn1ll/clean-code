using Core.Entities;

namespace Application.Interfaces;

public interface IMinioService
{
    Task<Result<bool>> CreateDocument(long documentId);
    Task<Result<string>> PullDocument(long documentId); 
    Task<Result<bool>> PushDocument(long documentId, string content);
}