using ProductsCacheDemo.Common.Constants;
using ProductsCacheDemo.Common.Interfaces;
using ProductsCacheDemo.Features.Products.Dtos;

namespace ProductsCacheDemo.Features.Products.Queries
{
    public record GetAllProductsQuery : ICacheableQuery<List<ProductDto>>
    {
        public string CacheKey => CacheKeys.ProductsAll;

        public TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(2);

        public TimeSpan? AbsoluteExpirationRelativeToNow => TimeSpan.FromMinutes(10);
    }
}
