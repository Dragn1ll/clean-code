using Application.Interfaces.Services;
using WebApi.Contracts.Users;
using WebApi.Switches;

namespace WebApi.Endpoints;

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
        
        return registerResult.IsSuccess ? Results.Ok() : ErrorSwitcher.SwitchError(registerResult.Error!);
    }

    private static async Task<IResult> Login(LoginUserRequest request, IUserService userService, 
        HttpContext context)
    {
            var loginResult = await userService.Login(request.Email, request.Password);

        if (!loginResult.IsSuccess) return ErrorSwitcher.SwitchError(loginResult.Error!);
        
        context.Response.Cookies.Append("not-jwt-token", loginResult.Value!);
        
        return Results.Ok();

    }
}