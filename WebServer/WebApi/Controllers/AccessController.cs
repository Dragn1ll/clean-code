using System.Security.Claims;
using Application.Dto;
using Application.Interfaces.Services;
using Core.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts.Accesses;
using WebApi.Filters.Accesses;
using WebApi.Switches;

namespace WebApi.Controllers;

[ApiController]
[Route("api/access")]
[Authorize]
public class AccessController : ControllerBase
{
    [HttpGet("documents")]
    public async Task<IResult> GetDocuments(IDocumentService documentService)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        
        var getDocumentsResult = await documentService.GetUserPermission(userId);
        if (!getDocumentsResult.IsSuccess)
            return ErrorSwitcher.SwitchError(getDocumentsResult.Error!);
        
        var documents = getDocumentsResult.Value!
            .Select(document => new DocumentListDto 
                { DocumentId = document.Id, Title = document.Title, Created = document.CreationDate }).ToList();

        return Results.Ok(documents);
    }

    [HttpPost("create")]
    [ServiceFilter(typeof(CreateSetAccessFilter))]
    public async Task<IResult> Create([FromBody] CreateSetAccessRequest request, IUserService userService,
        IAccessService accessService)
    {
        var getEmailResult = await userService.GetByEmail(request.UserEmail);
        if (!getEmailResult.IsSuccess)
            return ErrorSwitcher.SwitchError(getEmailResult.Error!);
        
        var createResult = await accessService.Create(getEmailResult.Value!.Id, 
            request.DocumentId, (Permissions)request.PermissionId);
        return createResult.IsSuccess 
            ? Results.Ok() 
            : ErrorSwitcher.SwitchError(createResult.Error!);
    }

    [HttpDelete("delete")]
    [ServiceFilter(typeof(DeleteAccessFilter))]
    public async Task<IResult> Delete([FromBody] DeleteAccessRequest request, IUserService userService,
        IAccessService accessService)
    {
        var getEmailResult = await userService.GetByEmail(request.UserEmail);
        if (!getEmailResult.IsSuccess)
            return ErrorSwitcher.SwitchError(getEmailResult.Error!);
        
        var deleteResult = await accessService.Delete(request.DocumentId, getEmailResult.Value!.Id);
        return deleteResult.IsSuccess 
            ? Results.Ok() 
            : ErrorSwitcher.SwitchError(deleteResult.Error!);
    }

    [HttpPost("set")]
    [ServiceFilter(typeof(CreateSetAccessFilter))]
    public async Task<IResult> Set([FromBody] CreateSetAccessRequest request, IUserService userService,
        IAccessService accessService)
    {
        var getEmailResult = await userService.GetByEmail(request.UserEmail);
        if (!getEmailResult.IsSuccess)
            return ErrorSwitcher.SwitchError(getEmailResult.Error!);
        
        var setAccessResult = await accessService.Set(getEmailResult.Value!.Id, request.DocumentId, 
            (Permissions)request.PermissionId);
        return setAccessResult.IsSuccess 
            ? Results.Ok()
            : ErrorSwitcher.SwitchError(setAccessResult.Error!);
    }

    [HttpGet("get/users")]
    [ServiceFilter(typeof(GetAccessFilter))]
    public async Task<IResult> GetUsers([FromQuery] GetAccessRequest request, IAccessService accessService)
    {
        var getUsersAccessResult = await accessService.GetUsers(request.DocumentId);
        if (!getUsersAccessResult.IsSuccess)
            return ErrorSwitcher.SwitchError(getUsersAccessResult.Error!);
        
        var users = new List<AccessDto>();

        foreach (var user in getUsersAccessResult.Value!)
        {
            users.Add(new AccessDto(user.Id, user.Email, 
                (await accessService.Check(user.Id, request.DocumentId)).Value!.Permission.ToString()));
        }
        
        return Results.Ok(users);
    }

    [HttpGet("get/readers")]
    [ServiceFilter(typeof(GetAccessFilter))]
    public async Task<IResult> GetReaders([FromQuery] GetAccessRequest request, IAccessService accessService)
    {
        var getUsersAccessResult = await accessService.GetReaders(request.DocumentId);
        if (!getUsersAccessResult.IsSuccess)
            return ErrorSwitcher.SwitchError(getUsersAccessResult.Error!);
        
        var users = new List<AccessDto>();

        foreach (var user in getUsersAccessResult.Value!)
        {
            users.Add(new AccessDto(user.Id, user.Email, 
                (await accessService.Check(user.Id, request.DocumentId)).Value!.Permission.ToString()));
        }
        
        return Results.Ok(users);
    }

    [HttpGet("get/writers")]
    [ServiceFilter(typeof(GetAccessFilter))]
    public async Task<IResult> GetWriters([FromQuery] GetAccessRequest request, IAccessService accessService)
    {
        var getUsersAccessResult = await accessService.GetWriters(request.DocumentId);
        if (!getUsersAccessResult.IsSuccess)
            return ErrorSwitcher.SwitchError(getUsersAccessResult.Error!);
        
        var users = new List<AccessDto>();

        foreach (var user in getUsersAccessResult.Value!)
        {
            users.Add(new AccessDto(user.Id, user.Email, 
                (await accessService.Check(user.Id, request.DocumentId)).Value!.Permission.ToString()));
        }
        
        return Results.Ok(users);
    }
}