namespace ProductCatalogApi.Business.Results
{
    public class Result<T> : Result
    {
        private readonly T? _value;
        public T? Value => _value;
        public Result(T? value, Error? error) : base(error)
        {
            _value = value;
        }
        public static Result<T> Success(T value) => new(value, null);
        public static new Result<T> Failure(Error error) => new(default, error);
    }


    public class Result
    {
        private readonly Error? _error;

        public bool IsSuccess => _error is null;

        public bool IsFailure => !IsSuccess;

        public Error? Error => _error;

        public Result(Error? error)
        {
            _error = error;
        }

        public static Result Success() => new(null);

        public static Result Failure(Error error) => new(error);
    }
}
