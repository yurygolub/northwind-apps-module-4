using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Northwind.Services.Products;

namespace Northwind.Services.InMemory.Products
{
    public class ProductManagementService : IProductManagementService
    {
        private readonly NorthwindContext northwindContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductManagementService"/> class.
        /// </summary>
        /// <param name="northwindContext">NorthwindContext.</param>
        /// <exception cref="ArgumentNullException">Thrown if northwindContext is null.</exception>
        public ProductManagementService(NorthwindContext northwindContext)
        {
            if (northwindContext is null)
            {
                throw new ArgumentNullException(nameof(northwindContext));
            }

            this.northwindContext = northwindContext;
        }

        public async Task<int> CreateProductAsync(Product product)
        {
            if (product is null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            await this.northwindContext.Products.AddAsync(MapProduct(product));
            await this.northwindContext.SaveChangesAsync();
            return product.Id;
        }

        public async Task<bool> DestroyProductAsync(int productId)
        {
            var product = await this.northwindContext.Products.FindAsync(productId);
            if (product != null)
            {
                this.northwindContext.Products.Remove(product);
                await this.northwindContext.SaveChangesAsync();
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

            var products = from product in this.northwindContext.Products
                           from name in names
                           where product.Name == name
                           select MapProduct(product);

            await foreach (var product in products.AsAsyncEnumerable())
            {
                yield return product;
            }
        }

        public async IAsyncEnumerable<Product> GetProductsAsync(int offset, int limit)
        {
            var products = this.northwindContext.Products
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
            var products = from product in this.northwindContext.Products
                           where product.CategoryId == categoryId
                           select MapProduct(product);

            await foreach (var product in products.AsAsyncEnumerable())
            {
                yield return product;
            }
        }

        public async Task<Product> GetProductAsync(int productId)
        {
            var contextProduct = await this.northwindContext.Products.FindAsync(productId);
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

            var contextProduct = await this.northwindContext.Products.FindAsync(productId);
            if (contextProduct is null)
            {
                return false;
            }

            contextProduct.CategoryId = product.CategoryId;
            contextProduct.Discontinued = product.Discontinued;
            contextProduct.Name = product.Name;
            contextProduct.QuantityPerUnit = product.QuantityPerUnit;
            contextProduct.ReorderLevel = product.ReorderLevel;
            contextProduct.SupplierId = product.SupplierId;
            contextProduct.UnitPrice = product.UnitPrice;
            contextProduct.UnitsInStock = product.UnitsInStock;
            contextProduct.UnitsOnOrder = product.UnitsOnOrder;

            await this.northwindContext.SaveChangesAsync();
            return true;
        }

        private static Product MapProduct(Entities.Product product)
        {
            return new Product()
            {
                Id = product.Id,
                CategoryId = product.CategoryId,
                Discontinued = product.Discontinued,
                Name = product.Name,
                QuantityPerUnit = product.QuantityPerUnit,
                ReorderLevel = product.ReorderLevel,
                SupplierId = product.SupplierId,
                UnitPrice = product.UnitPrice,
                UnitsInStock = product.UnitsInStock,
                UnitsOnOrder = product.UnitsOnOrder,
            };
        }

        private static Entities.Product MapProduct(Product product)
        {
            return new Entities.Product()
            {
                Id = product.Id,
                CategoryId = product.CategoryId,
                Discontinued = product.Discontinued,
                Name = product.Name,
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
