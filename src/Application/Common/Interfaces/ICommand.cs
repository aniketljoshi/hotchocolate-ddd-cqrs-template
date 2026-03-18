using MediatR;

namespace HotChocolateDddCqrsTemplate.Application.Common.Interfaces;

public interface ICommand<out TResponse> : IRequest<TResponse>
{
}
