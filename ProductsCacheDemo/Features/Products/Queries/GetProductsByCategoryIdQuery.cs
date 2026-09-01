using ProductsCacheDemo.Common.Interfaces;
using ProductsCacheDemo.Features.Products.Dtos;

namespace ProductsCacheDemo.Features.Products.Queries
{
    public record GetProductsByCategoryIdQuery(int CategoryId) : ICacheableQuery<List<ProductDto>>
    {
        public string CacheKey => $"category-{CategoryId}-products";

        public TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(5);
    }
}
