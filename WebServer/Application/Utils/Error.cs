using Core.Enum;

namespace Application.Utils;

public class Error(ErrorType errorType, string errorMessage)
{
    public ErrorType ErrorType { get; } = errorType;
    public string Message { get; } = errorMessage;
}