namespace ProductsCacheDemo.Features.Products.Dtos
{
    public record ProductDto(
        int Id,
        string Name,
        decimal Price,
        string Description,
        int CategoryId
        );
}
