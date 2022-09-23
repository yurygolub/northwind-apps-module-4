using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Northwind.DataAccess;
using Northwind.DataAccess.Products;
using Northwind.Services.Products;

#pragma warning disable S4457

namespace Northwind.Services.DataAccess.Products
{
    /// <summary>
    /// Represents a stub for a product management service.
    /// </summary>
    public sealed class ProductManagementService : IProductManagementService
    {
        private readonly IProductDataAccessObject dataAccessObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductManagementService"/> class.
        /// </summary>
        /// <param name="northwindDataAccessFactory">Factory for creating Northwind DAO.</param>
        public ProductManagementService(NorthwindDataAccessFactory northwindDataAccessFactory)
        {
            if (northwindDataAccessFactory is null)
            {
                throw new ArgumentNullException(nameof(northwindDataAccessFactory));
            }

            this.dataAccessObject = northwindDataAccessFactory.GetProductDataAccessObject();
        }

        /// <inheritdoc/>
        public async Task<int> CreateProductAsync(Product product)
        {
            if (product is null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            return await this.dataAccessObject.InsertProductAsync(MapProduct(product));
        }

        /// <inheritdoc/>
        public async Task<bool> DestroyProductAsync(int productId)
        {
            if (await this.dataAccessObject.DeleteProductAsync(productId))
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<Product> GetProductsByNameAsync(IEnumerable<string> names)
        {
            if (names is null)
            {
                throw new ArgumentNullException(nameof(names));
            }

            var products = this.dataAccessObject.SelectProductsByNameAsync(names);
            await foreach (var product in products)
            {
                yield return MapProduct(product);
            }
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<Product> GetProductsAsync(int offset, int limit)
        {
            var products = this.dataAccessObject.SelectProductsAsync(offset, limit);
            await foreach (var product in products)
            {
                yield return MapProduct(product);
            }
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<Product> GetProductsForCategoryAsync(int categoryId)
        {
            var products = this.dataAccessObject.SelectProductByCategoryAsync(new[] { categoryId });
            await foreach (var product in products)
            {
                yield return MapProduct(product);
            }
        }

        /// <inheritdoc/>
        public async Task<Product> GetProductAsync(int productId)
        {
            var productTransferObject = await this.dataAccessObject.FindProductAsync(productId);
            return MapProduct(productTransferObject);
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateProductAsync(int productId, Product product)
        {
            if (product is null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            if (await this.dataAccessObject.UpdateProductAsync(productId, MapProduct(product)))
            {
                return true;
            }

            return false;
        }

        private static Product MapProduct(ProductTransferObject productTransferObject)
        {
            return new Product()
            {
                Id = productTransferObject.Id,
                CategoryId = productTransferObject.CategoryId,
                Discontinued = productTransferObject.Discontinued,
                Name = productTransferObject.Name,
                QuantityPerUnit = productTransferObject.QuantityPerUnit,
                ReorderLevel = productTransferObject.ReorderLevel,
                SupplierId = productTransferObject.SupplierId,
                UnitPrice = productTransferObject.UnitPrice,
                UnitsInStock = productTransferObject.UnitsInStock,
                UnitsOnOrder = productTransferObject.UnitsOnOrder,
            };
        }

        private static ProductTransferObject MapProduct(Product product)
        {
            return new ProductTransferObject()
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
