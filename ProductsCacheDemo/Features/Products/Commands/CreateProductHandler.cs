using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using ProductsCacheDemo.Data;
using ProductsCacheDemo.Features.Products.Dtos;
using ProductsCacheDemo.Models;

namespace ProductsCacheDemo.Features.Products.Commands
{
    public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductDto>
    {
        private readonly AppDbContext _context;
        private readonly IDistributedCache _cache;

        public CreateProductHandler(IDistributedCache cache, AppDbContext context)
        {
            _cache = cache;
            _context = context;
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

            await _cache.RemoveAsync($"category-{product.CategoryId}-products", cancellationToken);

            await _cache.RemoveAsync($"product-all",cancellationToken);

            Console.WriteLine($"---> [REDIS CACHE INVALIDATED] Cleared 'category-{product.CategoryId}-products' and 'products-all'");

            return new ProductDto(
                product.Id,
                product.Name,
                product.Price,
                product.Description,
                product.CategoryId);
        }
    }
}
