namespace Application;

public class Result(bool isSuccess, Exception? error)
{
    public bool IsSuccess { get; } = isSuccess;
    public Exception? Error { get; } = error;
    
    public Result Success() => new (true, null);
    public Result Failure(Exception? error) => new (false, error);
}

public class Result<T>(bool isSuccess, Exception? error, T value) : Result(isSuccess, error)
{
    public T Value { get; } = value;
    
    
}