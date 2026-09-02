using ProductsCacheDemo.Common.Constants;
using ProductsCacheDemo.Common.Interfaces;
using ProductsCacheDemo.Features.Products.Dtos;

namespace ProductsCacheDemo.Features.Products.Queries
{
    public record GetProductsByCategoryIdQuery(int CategoryId) : ICacheableQuery<List<ProductDto>>
    {
        public string CacheKey => CacheKeys.CategoryProducts(CategoryId);

        public TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(5);

        public TimeSpan? AbsoluteExpirationRelativeToNow => TimeSpan.FromMinutes(15);
    }
}
