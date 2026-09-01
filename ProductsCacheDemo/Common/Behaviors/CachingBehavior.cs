using MediatR;
using Microsoft.Extensions.Caching.Memory;
using ProductsCacheDemo.Common.Interfaces;

namespace ProductsCacheDemo.Common.Behaviors
{
    public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : ICacheableQuery<TResponse>
    {
        private readonly IMemoryCache _cache;

        public CachingBehavior(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_cache.TryGetValue(request.CacheKey, out TResponse? cachedResponse) && cachedResponse is not null)
            {
                Console.WriteLine($"--->[CACHE HIT] Fetching data for key: '{request.CacheKey}' from Memory Cache.");
                return cachedResponse;
            }

            Console.WriteLine($"---> [CACHE MISS] Key: '{request.CacheKey}' not found. Executing DB Handler...");
            var response = await next();

            if (response is not null)
            {
                var options = new MemoryCacheEntryOptions();
                if (request.SlidingExpiration.HasValue)
                {
                    options.SetSlidingExpiration(request.SlidingExpiration.Value);
                }

                _cache.Set(request.CacheKey, response, options);
                Console.WriteLine($"---> [CACHE SAVED] Data for key: '{request.CacheKey}' saved to Memory Cache.");
            }

            return response;
                                   
        }
    }
}
