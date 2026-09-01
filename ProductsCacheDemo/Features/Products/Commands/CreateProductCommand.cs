using MediatR;
using ProductsCacheDemo.Common.Interfaces;
using ProductsCacheDemo.Features.Products.Dtos;

namespace ProductsCacheDemo.Features.Products.Commands
{
    public record CreateProductCommand(CreateProductDto Product) : IRequest<ProductDto>;
    
}
