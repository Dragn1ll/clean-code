namespace Application;

public class Result(bool isSuccess, Exception? error = null)
{
    public bool IsSuccess { get; } = isSuccess;
    public Exception? Error { get; } = error;
    
    public static Result Success() => new (true);
    public static Result Failure(Exception? error) => new (false, error);
}

public class Result<T>(bool isSuccess, Exception? error = null, T? value = default) : Result(isSuccess, error)
{
    public T? Value { get; } = value;
    
    public new static Result<T> Success(T value) => new (true, null, value);
    public new static Result<T> Failure(Exception? error) => new (false, error);
}