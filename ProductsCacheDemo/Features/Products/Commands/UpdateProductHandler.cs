using MediatR;
using Microsoft.Extensions.Caching.Memory;
using ProductsCacheDemo.Data;
using ProductsCacheDemo.Features.Products.Dtos;

namespace ProductsCacheDemo.Features.Products.Commands
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, ProductDto?>
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        public UpdateProductHandler(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<ProductDto?> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FindAsync(request.Id ,cancellationToken);
            if(product is null)
            {
                return null;
            }

            product.Name = request.product.Name;
            product.Price = request.product.Price;
            product.Description = request.product.Description;
            product.CategoryId = request.product.CategoryId;

            await _context.SaveChangesAsync(cancellationToken);

            //remove this item from cache 

            _cache.Remove($"product-{product.Id}");
            _cache.Remove($"products-all");

            Console.WriteLine($"---> [CACHE INVALIDATED] Cache cleared for 'product-{product.Id}' and 'products-all'");

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
