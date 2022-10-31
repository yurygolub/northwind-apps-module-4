using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Northwind.Services.Products;

namespace Northwind.Services.InMemory.Products
{
    public class ProductManagementService : IProductManagementService
    {
        private readonly NorthwindContext northwindContext;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductManagementService"/> class.
        /// </summary>
        /// <param name="northwindContext">NorthwindContext.</param>
        /// <param name="mapper">Mapper for entity mapping.</param>
        /// <exception cref="ArgumentNullException">Thrown if northwindContext is null.</exception>
        public ProductManagementService(NorthwindContext northwindContext, IMapper mapper)
        {
            if (northwindContext is null)
            {
                throw new ArgumentNullException(nameof(northwindContext));
            }

            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            this.northwindContext = northwindContext;
        }

        public async Task<int> CreateProductAsync(Product product)
        {
            if (product is null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            await this.northwindContext.Products.AddAsync(this.mapper.Map<Entities.Product>(product));
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
                           select this.mapper.Map<Product>(product);

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
                    .Select(p => this.mapper.Map<Product>(p));

            await foreach (var product in products.AsAsyncEnumerable())
            {
                yield return product;
            }
        }

        public async IAsyncEnumerable<Product> GetProductsForCategoryAsync(int categoryId)
        {
            var products = from product in this.northwindContext.Products
                           where product.CategoryId == categoryId
                           select this.mapper.Map<Product>(product);

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

            return this.mapper.Map<Product>(contextProduct);
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
    }
}
