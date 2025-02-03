using System.Security.Claims;
using Application.Dto;
using Application.Interfaces.Auth;
using Application.Interfaces.Services;
using Application.Utils;
using Core.Enum;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts.Users;
using WebApi.Switches;

namespace WebApi.Controllers;

[ApiController]
[Route("api/user")]
public class UserController(IUserService userService, IPasswordHasher passwordHasher) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IResult> Register([FromBody] RegisterUserRequest request, 
        [FromServices] IValidator<RegisterUserRequest> validator)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return Results.BadRequest(validationResult.Errors);
        
        var registerResult = await userService.Register(request.Name, request.Email, request.Password);
        
        return registerResult.IsSuccess 
            ? Results.Ok() 
            : ErrorSwitcher.SwitchError(registerResult.Error!);
    }

    [HttpPost("login")]
    public async Task<IResult> Login([FromBody] LoginUserRequest request)
    {
        var getResult = await userService.GetByEmail(request.Email); 
        if (!getResult.IsSuccess || getResult.Value == null)
            return ErrorSwitcher.SwitchError(new Error(ErrorType.BadRequest, 
                "Не существует аккаунта с таким email!"));
        
        var user = getResult.Value;
        var passwordIsValid = passwordHasher.Validate(request.Password, user!.Password);

        if (!passwordIsValid)
            return ErrorSwitcher.SwitchError(new Error(ErrorType.BadRequest, "Неправильный пароль!"));
        
        var loginResult = await userService.Login(user);

        if (!loginResult.IsSuccess) return ErrorSwitcher.SwitchError(loginResult.Error!);
        
        HttpContext.Response.Cookies.Append("jwt-cookies", loginResult.Value!);
        
        return Results.Ok();
    }

    [Authorize]
    [HttpGet("name")]
    public async Task<IResult> GetUserName()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        
        var getResult = await userService.GetById(userId);
        return !getResult.IsSuccess 
            ? ErrorSwitcher.SwitchError(getResult.Error!) 
            : Results.Ok(getResult.Value!.Name);
    }
    
    [Authorize]
    [HttpGet("documents")]
    public async Task<IResult> GetDocuments([FromServices] IDocumentService documentService)
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