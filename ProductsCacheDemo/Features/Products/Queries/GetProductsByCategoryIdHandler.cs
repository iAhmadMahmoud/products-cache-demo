using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductsCacheDemo.Data;
using ProductsCacheDemo.Features.Products.Dtos;

namespace ProductsCacheDemo.Features.Products.Queries
{
    public class GetProductsByCategoryIdHandler : IRequestHandler<GetProductsByCategoryIdQuery, List<ProductDto>>
    {
        private readonly AppDbContext _context;

        public GetProductsByCategoryIdHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductDto>> Handle(GetProductsByCategoryIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Products
                .AsNoTracking()
                .Where(x=>x.CategoryId == request.CategoryId)
                .Select(p =>new ProductDto(
                    p.Id,
                    p.Name,
                    p.Price,
                    p.Description,
                    p.CategoryId))
                .ToListAsync(cancellationToken);
        }
    }
}
