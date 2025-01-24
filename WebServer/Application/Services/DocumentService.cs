using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Utils;
using Core.Models;
using Infrastructure.Services;
using Infrastructure.Services.Options;
using Microsoft.Extensions.Options;

namespace Application.Services;

public class DocumentService(
    IDocumentsRepository documentsRepository,
    IMinioService minioService,
    IOptions<MinioOptions> minioConfig)
    : IDocumentService
{
    public async Task<Result> Create(Guid userId, string title)
    {
        var result = await documentsRepository.Create(userId, title);
        
        if (!result.IsSuccess)
            return result;

        try
        {
            await minioService.CreateDocument(result.Value);
        }
        catch (Exception exception)
        {
            await documentsRepository.Delete(result.Value);
            return Result.Failure(exception);
        }
        
        return Result.Success();
    }

    public async Task<Result> Delete(Guid documentId)
    {
        var deleteResult = await documentsRepository.Delete(documentId);
        if (!deleteResult.IsSuccess)
            return deleteResult;

        try
        {
            await minioService.DeleteDocument(documentId);
        }
        catch (Exception exception)
        {
            return Result.Failure(exception);
        }
        
        return Result.Success();
    }

    public async Task<Result> Rename(Guid documentId, string newTitle)
    {
        return await documentsRepository.Rename(documentId, newTitle);
    }

    public async Task<Result<MdDocument>> Get(Guid documentId)
    {
        return await documentsRepository.GetById(documentId);
    }

    public async Task<Result<IEnumerable<MdDocument>>> GetUserDocuments(Guid userId)
    {
        return await documentsRepository.GetByUser(userId);
    }

    public async Task<Result<IEnumerable<MdDocument>>> GetUserPermission(Guid userId)
    {
        return await documentsRepository.GetByUserPermission(userId);
    }
}