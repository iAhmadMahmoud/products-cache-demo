using MediatR;

namespace ProductsCacheDemo.Features.Products.Commands
{
    public record DeleteProductCommand(int Id) : IRequest<bool>;
}
