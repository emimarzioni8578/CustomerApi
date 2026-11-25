namespace Catalog.Application.Results;

public class Result : IFailureFactory<Result>
{
    protected Result(Error? error)
    {
        Error = error;
    }

    public bool IsSuccess => Error is null;

    public bool IsFailure => !IsSuccess;

    public Error? Error { get; }

    public static Result Success() => new(null);

    public static Result Failure(Error error) => new(error);
}

public class Result<T> : Result, IFailureFactory<Result<T>>
{
    private Result(T? value, Error? error)
        : base(error)
    {
        Value = value;
    }

    public T? Value { get; }

    public static Result<T> Success(T value) => new(value, null);

    public static new Result<T> Failure(Error error) => new(default, error);
}
