using Catalog.Application.Results;
using MediatR;

namespace Catalog.Application.Abstractions.Messaging;

public interface ICommand : IRequest<Result>;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>;
