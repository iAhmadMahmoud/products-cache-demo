using ProductsCacheDemo.Common.Constants;
using ProductsCacheDemo.Common.Interfaces;
using ProductsCacheDemo.Features.Categories.Dtos;

namespace ProductsCacheDemo.Features.Categories.Queries
{
    public class GetAllCategoriesQuery : ICacheableQuery<List<CategoryDto>>
    {
        public string CacheKey => CacheKeys.CategoriesAll;

        public TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(2);

        public TimeSpan? AbsoluteExpirationRelativeToNow => TimeSpan.FromMinutes(10);
    }
}
