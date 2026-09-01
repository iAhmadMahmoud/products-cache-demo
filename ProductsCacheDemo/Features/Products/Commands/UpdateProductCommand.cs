using MediatR;
using ProductsCacheDemo.Features.Products.Dtos;

namespace ProductsCacheDemo.Features.Products.Commands
{
    public record UpdateProductCommand(int Id, UpdateProductDto product) : IRequest<ProductDto?>;

}
