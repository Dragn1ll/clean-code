using Application.Dto;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Utils;
using Core.Enum;
using Core.Models;

namespace Application.Services;

public class DocumentService(
    IDocumentsRepository documentsRepository,
    IMinioService minioService,
    IMdService mdService)
    : IDocumentService
{
    public async Task<Result> Check(Guid documentId)
    {
        var checkResult = await documentsRepository.Check(documentId);
        
        if (!checkResult.IsSuccess)
            return Result.Failure(checkResult.Error);
        
        if (checkResult.Value == false)
            return Result.Failure(new Error(ErrorType.NotFound, 
                "Такого документа не существует!"));
        
        return Result.Success();
    }
    
    public async Task<Result<Guid>> Create(Guid userId, string title)
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
            return Result<Guid>.Failure(new Error(ErrorType.ServerError, exception.Message));
        }
        
        return result;
    }

    public async Task<Result> Delete(Guid documentId)
    {
        var checkResult = await Check(documentId);
        if (!checkResult.IsSuccess)
            return Result.Failure(checkResult.Error);
        
        var deleteResult = await documentsRepository.Delete(documentId);
        if (!deleteResult.IsSuccess)
            return deleteResult;

        try
        {
            await minioService.DeleteDocument(documentId);
        }
        catch (Exception exception)
        {
            return Result.Failure(new Error(ErrorType.ServerError, exception.Message));
        }
        
        return Result.Success();
    }

    public async Task<Result<string>> ConvertToHtml(Guid documentId, string mdText)
    {
        var checkResult = await Check(documentId);
        if (!checkResult.IsSuccess)
            return Result<string>.Failure(checkResult.Error);
        
        var lines = mdText == string.Empty 
            ? [string.Empty] 
            : mdText.Split(Environment.NewLine);
        var htmlCode = (await mdService.ConvertToHtml(lines[0])).Value;

        for (var index = 1; index < lines.Length; index++)
        {
            var tmpResult = await mdService.ConvertToHtml(lines[index]);
            
            if (!tmpResult.IsSuccess)
                return tmpResult;
            
            htmlCode += Environment.NewLine + tmpResult.Value;
        }
        
        return Result<string>.Success(htmlCode!);
    }

    public async Task<Result> Rename(Guid documentId, string newTitle)
    {
        var checkResult = await Check(documentId);
        if (!checkResult.IsSuccess)
            return Result.Failure(checkResult.Error);
        
        return await documentsRepository.Rename(documentId, newTitle);
    }

    public async Task<Result<MdDocument>> Get(Guid documentId)
    {
        var checkResult = await Check(documentId);
        if (!checkResult.IsSuccess)
            return Result<MdDocument>.Failure(checkResult.Error);
        
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