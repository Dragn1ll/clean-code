using System.Security.Claims;
using Application.Dto;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts.Users;
using WebApi.Switches;

namespace WebApi.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    [HttpPost("register")]
    public async Task<IResult> Register([FromBody] RegisterUserRequest request, IUserService userService)
    {
        var registerResult = await userService.Register(request.Name, request.Email, request.Password);
        
        return registerResult.IsSuccess 
            ? Results.Ok() 
            : ErrorSwitcher.SwitchError(registerResult.Error!);
    }

    [HttpPost("login")]
    public async Task<IResult> Login([FromBody] LoginUserRequest request, IUserService userService)
    {
        var loginResult = await userService.Login(request.Email, request.Password);

        if (!loginResult.IsSuccess) return ErrorSwitcher.SwitchError(loginResult.Error!);
        
        HttpContext.Response.Cookies.Append("jwt-cookies", loginResult.Value!);
        
        return Results.Ok();
    }
    
    [Authorize]
    [HttpGet("documents")]
    public async Task<IResult> GetDocuments(IDocumentService documentService)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        
        var getDocumentsResult = await documentService.GetUserDocuments(userId);
        if (!getDocumentsResult.IsSuccess)
            return ErrorSwitcher.SwitchError(getDocumentsResult.Error!);
        
        var documents = getDocumentsResult.Value!
            .Select(document => new DocumentListDto 
                { DocumentId = document.Id, Title = document.Title, Created = document.CreationDate }).ToList();

        return Results.Ok(documents);
    }
}