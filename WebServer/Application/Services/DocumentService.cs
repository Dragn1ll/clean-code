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
    
    public async Task<Result<Guid>> Create(Guid userId, string title, IAccessService accessService)
    {
        var document = new MdDocument(Guid.NewGuid(), userId, title, DateTime.Now);
        
        try
        {
            await minioService.CreateDocument(document.Id);
        }
        catch (Exception exception)
        {
            await documentsRepository.Delete(document.Id);
            return Result<Guid>.Failure(new Error(ErrorType.ServerError, exception.Message));
        }
        
        var checkResult = await minioService.DocumentExists(document.Id);
        if (!checkResult.IsSuccess || checkResult.Value == false)
            return Result<Guid>.Failure(new Error(ErrorType.ServerError, 
                "Не удалось создать документ из-за неполадок на сервере!"));
        
        var result = await documentsRepository.Create(document);
        if (!result.IsSuccess)
            return Result<Guid>.Failure(result.Error);
        
        var createAccessResult = await accessService.Create(userId, document.Id, 
            Permissions.Master);
        return !createAccessResult.IsSuccess 
            ? Result<Guid>.Failure(createAccessResult.Error) 
            : Result<Guid>.Success(document.Id);
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
        
        var lines = string.IsNullOrEmpty(mdText)
            ? [string.Empty]
            : mdText.Split("\n");
        var htmlCode = (await mdService.ConvertToHtml(lines[0])).Value;

        for (var index = 1; index < lines.Length; index++)
        {
            var tmpResult = await mdService.ConvertToHtml(lines[index]);
            
            if (!tmpResult.IsSuccess)
                return tmpResult;
            
            htmlCode += "<br>" + tmpResult.Value;
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
        
        var checkFileResult = await minioService.DocumentExists(documentId);
        if (!checkResult.IsSuccess || checkFileResult.Value == false)
            return Result<MdDocument>.Failure(new Error(ErrorType.ServerError, 
                "Не удалось получить документ из-за неполадок на сервере!"));
        
        var getContentResult = await minioService.PullDocument(documentId);
        if (!getContentResult.IsSuccess)
            return Result<MdDocument>.Failure(getContentResult.Error!);
        
        var getResult = await documentsRepository.GetById(documentId);
        if (!getResult.IsSuccess)
            return Result<MdDocument>.Failure(getResult.Error);
        
        getResult.Value!.Text = getContentResult.Value!;

        return getResult;
    }

    public async Task<Result<IEnumerable<MdDocument>>> GetUserDocuments(Guid userId)
    {
        return await documentsRepository.GetByUser(userId);
    }

    public async Task<Result<IEnumerable<MdDocument>>> GetUserPermission(Guid userId)
    {
        return await documentsRepository.GetByUserPermission(userId);
    }

    public async Task<Result<string>> AddToMaket(Guid documentId, string htmlCode)
    {
        var getResult = await Get(documentId);
        return !getResult.IsSuccess 
            ? Result<string>.Failure(getResult.Error) 
            : Result<string>.Success($"""
                                     <!DOCTYPE html>
                                     <html lang="en">
                                     <head>
                                         <meta charset="UTF-8">
                                         <title>{getResult.Value!.Title}</title>
                                     </head>
                                     <body>
                                         {htmlCode}
                                     </body>
                                     </html>
                                     """);
    }
}