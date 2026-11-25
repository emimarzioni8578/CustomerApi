using MediatR;

namespace Catalog.Application.Abstractions.Messaging;

public interface IQuery<out TResponse> : IRequest<TResponse>;
