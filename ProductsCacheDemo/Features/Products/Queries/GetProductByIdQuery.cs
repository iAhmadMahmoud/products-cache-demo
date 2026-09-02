using ProductsCacheDemo.Common.Constants;
using ProductsCacheDemo.Common.Interfaces;
using ProductsCacheDemo.Features.Products.Dtos;

namespace ProductsCacheDemo.Features.Products.Queries
{
    public record GetProductByIdQuery(int Id) : ICacheableQuery<ProductDto?>
    {
        public string CacheKey => CacheKeys.Product(Id);

        public TimeSpan? SlidingExpiration => TimeSpan.FromSeconds(30);

        public TimeSpan? AbsoluteExpirationRelativeToNow => TimeSpan.FromMinutes(5);
    }
}
