using System.Security.Claims;
using Application.Dto;
using Application.Interfaces.Services;
using Application.Utils;
using Core.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts.Documents;
using WebApi.Filters.Documents;
using WebApi.Switches;

namespace WebApi.Controllers;

[ApiController]
[Route("api/document")]
[Authorize]
public class DocumentController(IDocumentService documentService) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IResult> Create([FromBody] CreateDocumentRequest request, 
        [FromServices] IAccessService accessService)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        
        var createDocumentResult = await documentService.Create(userId, request.Title, accessService);
        return !createDocumentResult.IsSuccess 
            ? ErrorSwitcher.SwitchError(createDocumentResult.Error!) 
            : Results.Ok(createDocumentResult.Value);
    }

    [HttpGet("get/{documentId:guid}")]
    [ServiceFilter(typeof(DocumentGetFilter))]
    public async Task<IResult> Get(Guid documentId, [FromServices] IMinioService minioService)
    {
        var getDocumentResult = await documentService.Get(documentId);
        if (!getDocumentResult.IsSuccess)
            return ErrorSwitcher.SwitchError(getDocumentResult.Error!);
        
        var convertResult = await documentService.ConvertToHtml(documentId, getDocumentResult.Value!.Text);
        return convertResult.IsSuccess 
            ? Results.Ok(new DocumentRedactorDto
            {
                Title = getDocumentResult.Value!.Title,
                Text = getDocumentResult.Value!.Text,
                ConvertedText = convertResult.Value!
            }) 
            : ErrorSwitcher.SwitchError(convertResult.Error!);
    }

    [HttpDelete("delete")]
    [ServiceFilter(typeof(DocumentDeleteFilter))]
    public async Task<IResult> Delete([FromBody] DocumentIdRequest request)
    {
        var deleteResult = await documentService.Delete(request.DocumentId);
        return !deleteResult.IsSuccess 
            ? ErrorSwitcher.SwitchError(deleteResult.Error!) 
            : Results.Ok();
    }

    [HttpPost("convert")]
    [ServiceFilter(typeof(DocumentChangeFilter))]
    public async Task<IResult> Convert([FromBody] ConvertDocumentRequest request, 
        [FromServices] IMinioService minioService)
    {
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
    [ServiceFilter(typeof(DocumentRenameFilter))]
    public async Task<IResult> Rename([FromBody] RenameDocumentRequest request)
    {
        var renameResult = await documentService.Rename(request.DocumentId, request.NewName);
        return renameResult.IsSuccess 
            ? Results.Ok() 
            : ErrorSwitcher.SwitchError(renameResult.Error!);
    }
}