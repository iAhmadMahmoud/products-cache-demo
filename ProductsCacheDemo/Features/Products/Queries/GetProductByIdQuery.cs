using ProductsCacheDemo.Common.Interfaces;
using ProductsCacheDemo.Features.Products.Dtos;

namespace ProductsCacheDemo.Features.Products.Queries
{
    public record GetProductByIdQuery(int Id) : ICacheableQuery<ProductDto?>
    {
        public string CacheKey => $"product-{Id}";

        public TimeSpan? SlidingExpiration => TimeSpan.FromSeconds(30);
    }
}
