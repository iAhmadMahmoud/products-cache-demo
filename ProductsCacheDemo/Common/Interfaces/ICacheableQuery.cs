using MediatR;

namespace ProductsCacheDemo.Common.Interfaces
{
    public interface ICacheableQuery<TResponse> : IRequest<TResponse>
    {
        string CacheKey { get; }
        TimeSpan? SlidingExpiration => null;
        TimeSpan? AbsoluteExpirationRelativeToNow => null;
    }
}
