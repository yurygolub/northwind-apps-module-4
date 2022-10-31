using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Northwind.Services.Products;

namespace Northwind.Services.EntityFrameworkCore.Products
{
    public class ProductManagementService : IProductManagementService
    {
        private readonly Models.NorthwindContext context;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductManagementService"/> class.
        /// </summary>
        public ProductManagementService(Models.NorthwindContext context, IMapper mapper)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<int> CreateProductAsync(Product product)
        {
            _ = product ?? throw new ArgumentNullException(nameof(product));

            await this.context.Products.AddAsync(this.mapper.Map<Models.Product>(product));
            await this.context.SaveChangesAsync();
            return product.Id;
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            var product = await this.context.Products.FindAsync(productId);
            if (product != null)
            {
                this.context.Products.Remove(product);

                var orderDetails = this.context.OrderDetails.Where(orderDet => orderDet.Product == product);
                this.context.OrderDetails.RemoveRange(orderDetails);

                await this.context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async IAsyncEnumerable<Product> GetProductsByNameAsync(IEnumerable<string> names)
        {
            _ = names ?? throw new ArgumentNullException(nameof(names));

            var products = from product in this.context.Products
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
            var products = this.context.Products
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
            var products = from product in this.context.Products
                           where product.CategoryId == categoryId
                           select this.mapper.Map<Product>(product);

            await foreach (var product in products.AsAsyncEnumerable())
            {
                yield return product;
            }
        }

        public async Task<Product> GetProductAsync(int productId)
        {
            var contextProduct = await this.context.Products.FindAsync(productId);
            if (contextProduct is null)
            {
                return null;
            }

            return this.mapper.Map<Product>(contextProduct);
        }

        public async Task<bool> UpdateProductAsync(int productId, Product product)
        {
            _ = product ?? throw new ArgumentNullException(nameof(product));

            var contextProduct = await this.context.Products.FindAsync(productId);
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

            await this.context.SaveChangesAsync();
            return true;
        }
    }
}
