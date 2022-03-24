using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Northwind.Services.Products;
using Context = Northwind.Services.EntityFrameworkCore.Models;

namespace Northwind.Services.EntityFrameworkCore.Products
{
    public class ProductManagementService : IProductManagementService
    {
        private readonly string connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductManagementService"/> class.
        /// </summary>
        public ProductManagementService(string connectionString)
        {
            if (connectionString is null)
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            this.connectionString = connectionString;
        }

        public async Task<int> CreateProductAsync(Product product)
        {
            if (product is null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            await using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);
            await db.Products.AddAsync(MapProduct(product));
            await db.SaveChangesAsync();
            return product.Id;
        }

        public async Task<bool> DestroyProductAsync(int productId)
        {
            await using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);

            var product = await db.Products.FindAsync(productId);
            if (product != null)
            {
                db.Products.Remove(product);
                await db.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async IAsyncEnumerable<Product> GetProductsByNameAsync(IEnumerable<string> names)
        {
            if (names is null)
            {
                throw new ArgumentNullException(nameof(names));
            }

            await using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);

            var products = from product in db.Products
                           from name in names
                           where product.ProductName == name
                           select MapProduct(product);

            await foreach (var product in products.AsAsyncEnumerable())
            {
                yield return product;
            }
        }

        public async IAsyncEnumerable<Product> GetProductsAsync(int offset, int limit)
        {
            await using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);

            var products = db.Products
                    .Skip(offset)
                    .Take(limit)
                    .Select(p => MapProduct(p));

            await foreach (var product in products.AsAsyncEnumerable())
            {
                yield return product;
            }
        }

        public async IAsyncEnumerable<Product> GetProductsForCategoryAsync(int categoryId)
        {
            await using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);

            var products = from product in db.Products
                           where product.CategoryId == categoryId
                           select MapProduct(product);

            await foreach (var product in products.AsAsyncEnumerable())
            {
                yield return product;
            }
        }

        public async Task<Product> GetProductAsync(int productId)
        {
            await using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);

            var contextProduct = await db.Products.FindAsync(productId);
            if (contextProduct is null)
            {
                return null;
            }

            return MapProduct(contextProduct);
        }

        public async Task<bool> UpdateProductAsync(int productId, Product product)
        {
            if (product is null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            await using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);

            var contextProduct = await db.Products.FindAsync(productId);
            if (contextProduct is null)
            {
                return false;
            }

            contextProduct.CategoryId = product.CategoryId;
            contextProduct.Discontinued = product.Discontinued;
            contextProduct.ProductName = product.Name;
            contextProduct.QuantityPerUnit = product.QuantityPerUnit;
            contextProduct.ReorderLevel = product.ReorderLevel;
            contextProduct.SupplierId = product.SupplierId;
            contextProduct.UnitPrice = product.UnitPrice;
            contextProduct.UnitsInStock = product.UnitsInStock;
            contextProduct.UnitsOnOrder = product.UnitsOnOrder;

            await db.SaveChangesAsync();
            return true;
        }

        private static Product MapProduct(Context.Product product)
        {
            return new Product()
            {
                Id = product.ProductId,
                CategoryId = product.CategoryId,
                Discontinued = product.Discontinued,
                Name = product.ProductName,
                QuantityPerUnit = product.QuantityPerUnit,
                ReorderLevel = product.ReorderLevel,
                SupplierId = product.SupplierId,
                UnitPrice = product.UnitPrice,
                UnitsInStock = product.UnitsInStock,
                UnitsOnOrder = product.UnitsOnOrder,
            };
        }

        private static Context.Product MapProduct(Product product)
        {
            return new Context.Product()
            {
                ProductId = product.Id,
                CategoryId = product.CategoryId,
                Discontinued = product.Discontinued,
                ProductName = product.Name,
                QuantityPerUnit = product.QuantityPerUnit,
                ReorderLevel = product.ReorderLevel,
                SupplierId = product.SupplierId,
                UnitPrice = product.UnitPrice,
                UnitsInStock = product.UnitsInStock,
                UnitsOnOrder = product.UnitsOnOrder,
            };
        }
    }
}
