using System.Collections.Generic;
using Northwind.Services.Products;

namespace Northwind.Services.Implementation.Products
{
    internal class NorthwindContext
    {
        public static List<Product> Products { get; set; } = new List<Product>()
        {
            new Product() { Id = 1, Name = "chai" },
            new Product() { Id = 2, Name = "meat" },
            new Product() { Id = 3, Name = "milk" },
        };

        public static List<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>()
        {
            new ProductCategory() { Id = 1, Name = "grocery" },
            new ProductCategory() { Id = 2, Name = "beverages" },
            new ProductCategory() { Id = 3, Name = "condiments" },
        };
    }
}
