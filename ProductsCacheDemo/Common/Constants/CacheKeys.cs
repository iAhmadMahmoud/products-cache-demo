namespace ProductsCacheDemo.Common.Constants;

public static class CacheKeys
{
    public static string Product(int id) => $"product-{id}";
    public static string ProductsAll => "products-all";
    public static string CategoryProducts(int categoryId) => $"category-{categoryId}-products";
    public static string CategoriesAll => "categories-all";
}
