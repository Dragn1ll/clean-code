using System.Security.Claims;
using Application.Interfaces.Services;
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
    public async Task<IResult> Create([FromBody]string title, 
        IDocumentService documentService, IAccessService accessService)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        
        var createDocumentResult = await documentService.Create(userId, title);
        if (!createDocumentResult.IsSuccess)
            return ErrorSwitcher.SwitchError(createDocumentResult.Error!);
        
        var createAccessResult = await accessService.Create(userId, createDocumentResult.Value, 
            Permissions.Master);
        return !createAccessResult.IsSuccess 
            ? ErrorSwitcher.SwitchError(createAccessResult.Error!) 
            : Results.Ok(createDocumentResult.Value);
    }

    [HttpGet("get")]
    public async Task<IResult> Get([FromQuery] Guid documentId, 
        IDocumentService documentService, IAccessService accessService)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        
        var checkAccessResult = await accessService.Check(userId, documentId);
        if (!checkAccessResult.IsSuccess)
            return ErrorSwitcher.SwitchError(checkAccessResult.Error!);
        if ((int)checkAccessResult.Value!.Permissions < (int)Permissions.Read)
            return Results.BadRequest("Недостаточно прав для просмотра документа!");
        
        var getResult = await documentService.Get(documentId);
        return getResult.IsSuccess 
            ? Results.Ok(getResult.Value) 
            : ErrorSwitcher.SwitchError(getResult.Error!);
    }

    [HttpDelete("delete")]
    public async Task<IResult> Delete([FromBody] Guid documentId,
        IDocumentService documentService, IAccessService accessService)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        
        var checkMasterResult = await accessService.CheckMaster(userId, documentId);
        if (!checkMasterResult.IsSuccess)
            return ErrorSwitcher.SwitchError(checkMasterResult.Error!);
        
        var deleteResult = await documentService.Delete(documentId);
        return !deleteResult.IsSuccess 
            ? ErrorSwitcher.SwitchError(deleteResult.Error!) 
            : Results.Ok();
    }

    [HttpPost("convert")]
    public async Task<IResult> Convert([FromBody] ConvertDocumentRequest request,
        IDocumentService documentService, IAccessService accessService)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

        var checkAccessResult = await accessService.Check(userId, request.DocumentId);
        if (!checkAccessResult.IsSuccess)
            return ErrorSwitcher.SwitchError(checkAccessResult.Error!);
        if ((int)checkAccessResult.Value!.Permissions < (int)Permissions.Write)
            return Results.BadRequest("Недостаточно прав для изменения документа!");
        
        var convertResult = await documentService.ConvertToHtml(request.DocumentId, request.Content);
        return checkAccessResult.IsSuccess 
            ? Results.Ok(convertResult.Value)
            : ErrorSwitcher.SwitchError(convertResult.Error!);
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