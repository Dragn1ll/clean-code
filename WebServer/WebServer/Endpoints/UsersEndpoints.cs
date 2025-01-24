using Application.Interfaces.Services;
using WebServer.Contracts.Users;

namespace WebServer.Endpoints;

public static class UsersEndpoints
{
    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("register", Register);
        
        app.MapPost("login", Login);
        
        return app;
    }

    private static async Task<IResult> Register(RegisterUserRequest request, IUserService userService)
    {
        var registerResult = await userService.Register(request.Name, request.Email, request.Password);
        
        if (!registerResult.IsSuccess)
            return Results.BadRequest(registerResult.Error!.Message);
        
        return Results.Ok();
    }

    private static async Task<IResult> Login(LoginUserRequest request, IUserService userService, 
        HttpContext context)
    {
        var loginResult = await userService.Login(request.Email, request.Password);

        if (!loginResult.IsSuccess) return Results.BadRequest("Неправильный пароль!");
        
        context.Response.Cookies.Append("", loginResult.Value!);
        
        return Results.Ok(loginResult.Value);

    }
}