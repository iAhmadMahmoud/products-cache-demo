using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductsCacheDemo.Data;
using ProductsCacheDemo.Features.Products.Dtos;

namespace ProductsCacheDemo.Features.Categories.Queries
{
    public class GetAllCategoriesHandler : IRequestHandler<GetAllCategoriesQuery, List<CategoryDto>>
    {
        private readonly AppDbContext _context;

        public GetAllCategoriesHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            return await _context.Categories
             .AsNoTracking()
             .Select(c => new CategoryDto(c.Id, c.Name))
             .ToListAsync(cancellationToken);
        }
    }
}
