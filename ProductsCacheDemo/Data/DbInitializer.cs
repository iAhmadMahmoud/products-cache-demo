using ProductsCacheDemo.Models;

namespace ProductsCacheDemo.Data
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext context)
        {
            if (context.Categories.Any())
            {
                return; 
            }

            var electronics = new Category { Id = 1, Name = "Electronics" };
            var accessories = new Category { Id = 2, Name = "Accessories" };
            var homeAppliances = new Category { Id = 3, Name = "Home Appliances" };

            context.Categories.AddRange(electronics, accessories, homeAppliances);


            var products = new List<Product>
        {
            new Product
            {
                Id = 1,
                Name = "Gaming Laptop",
                Price = 1500.00m,
                Description = "High-spec laptop with RTX GPU",
                CategoryId = 1
            },
            new Product
            {
                Id = 2,
                Name = "Smartphone",
                Price = 900.00m,
                Description = "Latest OLED display phone",
                CategoryId = 1
            },
            new Product
            {
                Id = 3,
                Name = "Wireless Mouse",
                Price = 30.00m,
                Description = "Ergonomic 2.4GHz mouse",
                CategoryId = 2
            },
            new Product
            {
                Id = 4,
                Name = "Mechanical Keyboard",
                Price = 75.00m,
                Description = "RGB Backlit Switches",
                CategoryId = 2
            },
            new Product
            {
                Id = 5,
                Name = "Coffee Maker",
                Price = 120.00m,
                Description = "Automatic Espresso Machine",
                CategoryId = 3
            }
        };

            context.Products.AddRange(products);


            context.SaveChanges();
        }
    }
}
