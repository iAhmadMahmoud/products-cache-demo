using ProductsCacheDemo.Common.Interfaces;
using ProductsCacheDemo.Features.Categories.Dtos;

namespace ProductsCacheDemo.Features.Categories.Queries
{
    public class GetAllCategoriesQuery : ICacheableQuery<List<CategoryDto>>
    {
        public string CacheKey => "categories-all";

        public TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(2);
    }
}
