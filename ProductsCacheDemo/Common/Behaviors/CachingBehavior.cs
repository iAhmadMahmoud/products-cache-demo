using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using ProductsCacheDemo.Common.Interfaces;

namespace ProductsCacheDemo.Common.Behaviors;

public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheableQuery<TResponse>
{
    private const string NullCacheSentinel = "$$NULL_ENTRY$$";
    private static readonly TimeSpan NullCacheExpiration = TimeSpan.FromSeconds(30);

    // Striped lock pool: fixes the unbounded memory leak of ConcurrentDictionary
    // while still preventing stampedes per key
    private const int LockPoolSize = 64;
    private static readonly SemaphoreSlim[] _locks = Enumerable.Range(0, LockPoolSize)
        .Select(_ => new SemaphoreSlim(1, 1))
        .ToArray();

    private readonly IDistributedCache _cache;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

    public CachingBehavior(IDistributedCache cache, ILogger<CachingBehavior<TRequest, TResponse>> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    private static SemaphoreSlim GetLock(string key)
    {
        var bucket = (uint)key.GetHashCode() % LockPoolSize;
        return _locks[bucket];
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var cacheKey = request.CacheKey;

        // 1. First cache check (outside lock)
        var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedData))
        {
            if (cachedData == NullCacheSentinel)
            {
                _logger.LogInformation("---> [REDIS CACHE HIT (NULL SENTINEL)] Key: '{CacheKey}'", cacheKey);
                return default!;
            }

            _logger.LogInformation("---> [REDIS CACHE HIT] Key: '{CacheKey}'", cacheKey);
            return JsonSerializer.Deserialize<TResponse>(cachedData)!;
        }

        // 2. Acquire striped lock to prevent Cache Stampede (Thundering Herd)
        var semaphore = GetLock(cacheKey);
        await semaphore.WaitAsync(cancellationToken);

        try
        {
            // Double-checked locking pattern
            cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);
            if (!string.IsNullOrEmpty(cachedData))
            {
                if (cachedData == NullCacheSentinel)
                {
                    _logger.LogInformation("---> [REDIS CACHE HIT (After Lock - NULL SENTINEL)] Key: '{CacheKey}'", cacheKey);
                    return default!;
                }

                _logger.LogInformation("---> [REDIS CACHE HIT (After Lock)] Key: '{CacheKey}'", cacheKey);
                return JsonSerializer.Deserialize<TResponse>(cachedData)!;
            }

            _logger.LogInformation("---> [REDIS CACHE MISS - Fetching from DB] Key: '{CacheKey}'", cacheKey);
            var response = await next();

            if (response is null)
            {
                // Prevent Cache Penetration: Cache null result with short TTL
                var nullOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = NullCacheExpiration
                };

                await _cache.SetStringAsync(cacheKey, NullCacheSentinel, nullOptions, cancellationToken);
                _logger.LogInformation("---> [REDIS CACHE SAVED (NULL SENTINEL)] Key: '{CacheKey}'", cacheKey);
                return response;
            }

            var options = new DistributedCacheEntryOptions();
            if (request.SlidingExpiration.HasValue)
            {
                options.SetSlidingExpiration(request.SlidingExpiration.Value);
            }
            if (request.AbsoluteExpirationRelativeToNow.HasValue)
            {
                options.SetAbsoluteExpiration(request.AbsoluteExpirationRelativeToNow.Value);
            }

            var serializedData = JsonSerializer.Serialize(response);
            await _cache.SetStringAsync(cacheKey, serializedData, options, cancellationToken);

            _logger.LogInformation("---> [REDIS CACHE SAVED] Key: '{CacheKey}'", cacheKey);
            return response;
        }
        finally
        {
            semaphore.Release();
        }
    }
}