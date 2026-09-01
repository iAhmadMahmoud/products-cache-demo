namespace ProductsCacheDemo.Features.Products.Dtos
{
    public record CreateProductDto(string Name, decimal Price, string Description, int CategoryId);
        
}
