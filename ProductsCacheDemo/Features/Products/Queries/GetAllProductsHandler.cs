using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductsCacheDemo.Data;
using ProductsCacheDemo.Features.Products.Dtos;

namespace ProductsCacheDemo.Features.Products.Queries
{
    public class GetAllProductsHandler : IRequestHandler<GetAllProductsQuery, List<ProductDto>>
    {
        private readonly AppDbContext _context;

        public GetAllProductsHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Products
                .AsNoTracking()
                .Select(p => new ProductDto(
                    p.Id,
                    p.Name,
                    p.Price,
                    p.Description,
                    p.CategoryId
                ))
                .ToListAsync(cancellationToken);
        }
    }
}
