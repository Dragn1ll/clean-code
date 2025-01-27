using System.Security.Claims;
using Application.Dto;
using Application.Interfaces.Services;
using Application.Utils;
using Core.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts.Documents;
using WebApi.Switches;

namespace WebApi.Controllers;

[ApiController]
[Route("api/document")]
[Authorize]
public class DocumentController : ControllerBase
{
    [HttpPost("create")]
    public async Task<IResult> Create([FromBody] CreateDocumentRequest request, 
        IDocumentService documentService, IAccessService accessService)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        
        var createDocumentResult = await documentService.Create(userId, request.Title);
        if (!createDocumentResult.IsSuccess)
            return ErrorSwitcher.SwitchError(createDocumentResult.Error!);
        
        var createAccessResult = await accessService.Create(userId, createDocumentResult.Value, 
            Permissions.Master);
        return !createAccessResult.IsSuccess 
            ? ErrorSwitcher.SwitchError(createAccessResult.Error!) 
            : Results.Ok(createDocumentResult.Value);
    }

    [HttpGet("get")]
    public async Task<IResult> Get([FromQuery] DocumentIdRequest request, 
        IMinioService minioService, IDocumentService documentService, IAccessService accessService)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        
        var checkAccessResult = await accessService.Check(userId, request.DocumentId);
        if (!checkAccessResult.IsSuccess)
            return ErrorSwitcher.SwitchError(checkAccessResult.Error!);
        if ((int)checkAccessResult.Value!.Permission < (int)Permissions.Read)
            return Results.BadRequest("Недостаточно прав для просмотра документа!");
        
        var getContentResult = await minioService.PullDocument(request.DocumentId);
        if (!getContentResult.IsSuccess)
            return ErrorSwitcher.SwitchError(getContentResult.Error!);
        
        var getDocumentInfoResult = await documentService.Get(request.DocumentId);
        if (!getDocumentInfoResult.IsSuccess)
            return ErrorSwitcher.SwitchError(getDocumentInfoResult.Error!);
        
        var convertResult = await documentService.ConvertToHtml(request.DocumentId, getContentResult.Value!);
        return convertResult.IsSuccess 
            ? Results.Ok(new DocumentRedactorDto
            {
                Title = getDocumentInfoResult.Value!.Title,
                Text = getContentResult.Value!,
                ConvertedText = convertResult.Value!
            }) 
            : ErrorSwitcher.SwitchError(convertResult.Error!);
    }

    [HttpDelete("delete")]
    public async Task<IResult> Delete([FromBody] DocumentIdRequest request,
        IDocumentService documentService, IAccessService accessService)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        
        var checkMasterResult = await accessService.CheckMaster(userId, request.DocumentId);
        if (!checkMasterResult.IsSuccess)
            return ErrorSwitcher.SwitchError(checkMasterResult.Error!);
        
        var deleteResult = await documentService.Delete(request.DocumentId);
        return !deleteResult.IsSuccess 
            ? ErrorSwitcher.SwitchError(deleteResult.Error!) 
            : Results.Ok();
    }

    [HttpPost("convert")]
    public async Task<IResult> Convert([FromBody] ConvertDocumentRequest request,
        IDocumentService documentService, IMinioService minioService, IAccessService accessService)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

        var checkAccessResult = await accessService.Check(userId, request.DocumentId);
        if (!checkAccessResult.IsSuccess)
            return ErrorSwitcher.SwitchError(checkAccessResult.Error!);
        if ((int)checkAccessResult.Value!.Permission < (int)Permissions.Write)
            return Results.BadRequest("Недостаточно прав для изменения документа!");
        
        var convertResult = await documentService.ConvertToHtml(request.DocumentId, request.Content);
        if (!convertResult.IsSuccess)
            return ErrorSwitcher.SwitchError(convertResult.Error!);
        
        var pushResult = await minioService.PushDocument(request.DocumentId, request.Content);
        return pushResult.IsSuccess 
            ? Results.Ok(convertResult.Value)
            : ErrorSwitcher.SwitchError(new Error(ErrorType.ServerError, 
                "Не удалось загрузить получившийся документ в систему..."));
    }

    [HttpPost("rename")]
    public async Task<IResult> Rename([FromBody] RenameDocumentRequest request,
        IDocumentService documentService, IAccessService accessService)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        
        var checkMasterResult = await accessService.CheckMaster(userId, request.DocumentId);
        if (!checkMasterResult.IsSuccess)
            return ErrorSwitcher.SwitchError(checkMasterResult.Error!);
        
        var renameResult = await documentService.Rename(request.DocumentId, request.NewName);
        return renameResult.IsSuccess 
            ? Results.Ok() 
            : ErrorSwitcher.SwitchError(renameResult.Error!);
    }
}