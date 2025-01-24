using Application.Utils;
using Core.Enum;

namespace WebServer.Switches;

public static class ErrorSwitcher
{
    public static IResult SwitchError(Error error)
    {
        return error.ErrorType switch
        {
            ErrorType.BadRequest => Results.BadRequest(error.Message),
            ErrorType.NotFound => Results.NotFound(error.Message),
            ErrorType.SignOut => Results.SignOut(),
            _ => Results.StatusCode(500)
        };
    }
}