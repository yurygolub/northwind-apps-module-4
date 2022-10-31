using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Northwind.Services.Products;
using Context = Northwind.Services.EntityFrameworkCore.Models;

namespace Northwind.Services.EntityFrameworkCore.Products
{
    public class ProductManagementService : IProductManagementService
    {
        private readonly string connectionString;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductManagementService"/> class.
        /// </summary>
        public ProductManagementService(string connectionString, IMapper mapper)
        {
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<int> CreateProductAsync(Product product)
        {
            _ = product ?? throw new ArgumentNullException(nameof(product));

            await using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);
            await db.Products.AddAsync(this.mapper.Map<Models.Product>(product));
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

                var orderDetails = db.OrderDetails.Where(orderDet => orderDet.Product == product);
                db.OrderDetails.RemoveRange(orderDetails);

                await db.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async IAsyncEnumerable<Product> GetProductsByNameAsync(IEnumerable<string> names)
        {
            _ = names ?? throw new ArgumentNullException(nameof(names));

            await using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);

            var products = from product in db.Products
                           from name in names
                           where product.ProductName == name
                           select this.mapper.Map<Product>(product);

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
                .Select(p => this.mapper.Map<Product>(p));

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
                           select this.mapper.Map<Product>(product);

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

            return this.mapper.Map<Product>(contextProduct);
        }

        public async Task<bool> UpdateProductAsync(int productId, Product product)
        {
            _ = product ?? throw new ArgumentNullException(nameof(product));

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
    }
}
