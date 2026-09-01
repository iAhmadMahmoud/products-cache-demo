using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Pipelines.Sockets.Unofficial;
using ProductsCacheDemo.Common.Interfaces;
using System.Text.Json;

namespace ProductsCacheDemo.Common.Behaviors
{
    public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : ICacheableQuery<TResponse>
    {
       
        private readonly IDistributedCache _cache;

        public CachingBehavior(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var cachedData = await _cache.GetStringAsync(request.CacheKey, cancellationToken);

            if (!string.IsNullOrEmpty(cachedData))
            {
                Console.WriteLine($"---> [REDIS CACHE HIT] Key: '{request.CacheKey}'");
                return JsonSerializer.Deserialize<TResponse>(cachedData)!;
            }

            Console.WriteLine($"---> [REDIS CACHE MISS] Key: '{request.CacheKey}'");
            var response = await next();

            if (response is not null)
            {
                var options = new DistributedCacheEntryOptions();
                if (request.SlidingExpiration.HasValue)
                {
                    options.SetSlidingExpiration(request.SlidingExpiration.Value);
                }

                var serializedData = JsonSerializer.Serialize(response);
                await _cache.SetStringAsync(request.CacheKey, serializedData,options, cancellationToken);

                Console.WriteLine($"---> [REDIS CACHE SAVED] Key: '{request.CacheKey}'");
            }

            return response;

                       
        }
    }
}
