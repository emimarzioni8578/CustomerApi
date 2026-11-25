namespace Catalog.Application.Results;

// Permite que código genérico (ValidationBehavior) cree un Result o un Result<T> fallido sin reflexión.
public interface IFailureFactory<TResult>
    where TResult : Result
{
    static abstract TResult Failure(Error error);
}
