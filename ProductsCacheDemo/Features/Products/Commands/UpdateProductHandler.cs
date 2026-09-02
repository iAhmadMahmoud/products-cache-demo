using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using ProductsCacheDemo.Common.Constants;
using ProductsCacheDemo.Data;
using ProductsCacheDemo.Features.Products.Dtos;

namespace ProductsCacheDemo.Features.Products.Commands
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, ProductDto?>
    {
        private readonly AppDbContext _context;
        private readonly IDistributedCache _cache;
        private readonly ILogger<UpdateProductHandler> _logger;

        public UpdateProductHandler(
            AppDbContext context,
            IDistributedCache cache,
            ILogger<UpdateProductHandler> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ProductDto?> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FindAsync(request.Id, cancellationToken);
            if (product is null)
            {
                return null;
            }

            var oldCategoryId = product.CategoryId;

            product.Name = request.product.Name;
            product.Price = request.product.Price;
            product.Description = request.product.Description;
            product.CategoryId = request.product.CategoryId;

            await _context.SaveChangesAsync(cancellationToken);

            // Invalidate product specific and all-products cache
            var productKey = CacheKeys.Product(product.Id);
            var productsAllKey = CacheKeys.ProductsAll;
            var oldCategoryKey = CacheKeys.CategoryProducts(oldCategoryId);

            await _cache.RemoveAsync(productKey, cancellationToken);
            await _cache.RemoveAsync(productsAllKey, cancellationToken);
            await _cache.RemoveAsync(oldCategoryKey, cancellationToken);

            if (oldCategoryId != product.CategoryId)
            {
                var newCategoryKey = CacheKeys.CategoryProducts(product.CategoryId);
                await _cache.RemoveAsync(newCategoryKey, cancellationToken);
                _logger.LogInformation("---> [REDIS CACHE INVALIDATED] Cleared '{ProductKey}', '{ProductsAllKey}', '{OldCategoryKey}', and '{NewCategoryKey}'",
                    productKey, productsAllKey, oldCategoryKey, newCategoryKey);
            }
            else
            {
                _logger.LogInformation("---> [REDIS CACHE INVALIDATED] Cleared '{ProductKey}', '{ProductsAllKey}', and '{OldCategoryKey}'",
                    productKey, productsAllKey, oldCategoryKey);
            }

            return new ProductDto(
                product.Id,
                product.Name,
                product.Price,
                product.Description,
                product.CategoryId
            );
        }
    }
}
