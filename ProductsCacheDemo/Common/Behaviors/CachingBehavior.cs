using System.Collections.Concurrent;
using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using ProductsCacheDemo.Common.Interfaces;

namespace ProductsCacheDemo.Common.Behaviors;

public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheableQuery<TResponse>
{
    private readonly IDistributedCache _cache;

    private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

    public CachingBehavior(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var cacheKey = request.CacheKey;

        var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedData))
        {
            Console.WriteLine($"---> [REDIS CACHE HIT] Key: '{cacheKey}'");
            return JsonSerializer.Deserialize<TResponse>(cachedData)!;
        }

        var semaphore = _locks.GetOrAdd(cacheKey, _ => new SemaphoreSlim(1, 1));

        await semaphore.WaitAsync(cancellationToken);

        try
        {
            cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);
            if (!string.IsNullOrEmpty(cachedData))
            {
                Console.WriteLine($"---> [REDIS CACHE HIT (After Lock)] Key: '{cacheKey}'");
                return JsonSerializer.Deserialize<TResponse>(cachedData)!;
            }

            Console.WriteLine($"---> [REDIS CACHE MISS - Fetching from DB] Key: '{cacheKey}'");
            var response = await next();

            if (response is not null)
            {
                var options = new DistributedCacheEntryOptions();
                if (request.SlidingExpiration.HasValue)
                {
                    options.SetSlidingExpiration(request.SlidingExpiration.Value);
                }

                var serializedData = JsonSerializer.Serialize(response);
                await _cache.SetStringAsync(cacheKey, serializedData, options, cancellationToken);

                Console.WriteLine($"---> [REDIS CACHE SAVED] Key: '{cacheKey}'");
            }

            return response;
        }
        finally
        {
            semaphore.Release();
        }
    }
}