using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using ProductsCacheDemo.Common.Constants;
using ProductsCacheDemo.Data;
using ProductsCacheDemo.Features.Products.Dtos;
using ProductsCacheDemo.Models;

namespace ProductsCacheDemo.Features.Products.Commands
{
    public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductDto>
    {
        private readonly AppDbContext _context;
        private readonly IDistributedCache _cache;
        private readonly ILogger<CreateProductHandler> _logger;

        public CreateProductHandler(
            IDistributedCache cache,
            AppDbContext context,
            ILogger<CreateProductHandler> logger)
        {
            _cache = cache;
            _context = context;
            _logger = logger;
        }

        public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Name = request.Product.Name,
                Price = request.Product.Price,
                Description = request.Product.Description,
                CategoryId = request.Product.CategoryId
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync(cancellationToken);

            var categoryKey = CacheKeys.CategoryProducts(product.CategoryId);
            var productsAllKey = CacheKeys.ProductsAll;

            await _cache.RemoveAsync(categoryKey, cancellationToken);
            await _cache.RemoveAsync(productsAllKey, cancellationToken);

            _logger.LogInformation("---> [REDIS CACHE INVALIDATED] Cleared '{CategoryKey}' and '{ProductsAllKey}'", categoryKey, productsAllKey);

            return new ProductDto(
                product.Id,
                product.Name,
                product.Price,
                product.Description,
                product.CategoryId);
        }
    }
}
