using MediatR;
using NetCoreKit.Domain;

namespace NetCoreKit.Infrastructure.AspNetCore.CleanArch
{
  public interface IEventHandler<in TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
  {
    IQueryRepositoryFactory QueryRepositoryFactory { get; }
  }

  public interface ITxEventHandler<in TRequest, TResponse> : IEventHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
  {
    IUnitOfWorkAsync UnitOfWork { get; }
  }
}
