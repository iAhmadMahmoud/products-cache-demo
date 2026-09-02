using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using ProductsCacheDemo.Common.Constants;
using ProductsCacheDemo.Data;

namespace ProductsCacheDemo.Features.Products.Commands
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly AppDbContext _context;
        private readonly IDistributedCache _cache;
        private readonly ILogger<DeleteProductHandler> _logger;

        public DeleteProductHandler(
            AppDbContext context,
            IDistributedCache cache,
            ILogger<DeleteProductHandler> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FindAsync(request.Id, cancellationToken);
            if (product is null)
            {
                return false;
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync(cancellationToken);

            var productKey = CacheKeys.Product(product.Id);
            var productsAllKey = CacheKeys.ProductsAll;
            var categoryKey = CacheKeys.CategoryProducts(product.CategoryId);

            try
            {
                await _cache.RemoveAsync(productKey, cancellationToken);
                await _cache.RemoveAsync(productsAllKey, cancellationToken);
                await _cache.RemoveAsync(categoryKey, cancellationToken);

                _logger.LogInformation("---> [REDIS CACHE INVALIDATED] Cleared '{ProductKey}', '{ProductsAllKey}', and '{CategoryKey}'",
                    productKey, productsAllKey, categoryKey);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "---> [REDIS CACHE ERROR] Failed to invalidate cache on product deletion");
            }

            return true;
        }
    }
}
