using System.Security.Claims;
using Application.Interfaces.Services;
using Core.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Switches;

namespace WebApi.Controllers;

[ApiController]
[Route("document")]
[Authorize]
public class DocumentController : ControllerBase
{
    [HttpGet]
    [Route("create")]
    public async Task<IResult> Create(string title, IDocumentService documentService, IAccessService accessService)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userId = Guid.Parse(userIdString!);
        
        var createDocumentResult = await documentService.Create(userId, title);
        if (!createDocumentResult.IsSuccess)
            return ErrorSwitcher.SwitchError(createDocumentResult.Error!);
        
        var createAccessResult = await accessService.Create(userId, createDocumentResult.Value, 
            Permissions.Master);
        if (!createAccessResult.IsSuccess)
            return ErrorSwitcher.SwitchError(createAccessResult.Error!);
        
        return Results.Ok(createDocumentResult.Value);
    }
}