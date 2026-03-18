using MediatR;

namespace HotChocolateDddCqrsTemplate.Application.Common.Interfaces;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}
