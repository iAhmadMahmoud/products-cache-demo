using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductsCacheDemo.Data;
using ProductsCacheDemo.Features.Products.Dtos;

namespace ProductsCacheDemo.Features.Products.Queries
{
    public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
    {
        private readonly AppDbContext _context;

        public GetProductByIdHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (product == null)
            {
                return null;
            }
            return new ProductDto(
            product.Id,
            product.Name,
            product.Price,
            product.Description,
            product.CategoryId
            );
        }
    }
}