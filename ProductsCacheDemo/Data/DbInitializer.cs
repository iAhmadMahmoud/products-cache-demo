using ProductsCacheDemo.Models;

namespace ProductsCacheDemo.Data;

public static class DbInitializer
{
    public static void Seed(AppDbContext context)
    {
        if (context.Categories.Any())
        {
            return;
        }

        var electronics = new Category { Name = "Electronics" };
        var accessories = new Category { Name = "Accessories" };
        var homeAppliances = new Category { Name = "Home Appliances" };

        var products = new List<Product>
        {
            new Product
            {
                Name = "Gaming Laptop",
                Price = 1500.00m,
                Description = "High-spec laptop with RTX GPU",
                Category = electronics
            },
            new Product
            {
                Name = "Smartphone",
                Price = 900.00m,
                Description = "Latest OLED display phone",
                Category = electronics
            },
            new Product
            {
                Name = "Wireless Mouse",
                Price = 30.00m,
                Description = "Ergonomic 2.4GHz mouse",
                Category = accessories
            },
            new Product
            {
                Name = "Mechanical Keyboard",
                Price = 75.00m,
                Description = "RGB Backlit Switches",
                Category = accessories
            },
            new Product
            {
                Name = "Coffee Maker",
                Price = 120.00m,
                Description = "Automatic Espresso Machine",
                Category = homeAppliances
            }
        };

       
        context.Categories.AddRange(electronics, accessories, homeAppliances);
        context.Products.AddRange(products);

        context.SaveChanges();
    }
}