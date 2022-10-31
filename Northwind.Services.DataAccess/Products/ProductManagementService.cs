using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
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
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductManagementService"/> class.
        /// </summary>
        /// <param name="northwindDataAccessFactory">Factory for creating Northwind DAO.</param>
        /// <param name="mapper">Mapper for entity mapping.</param>
        public ProductManagementService(NorthwindDataAccessFactory northwindDataAccessFactory, IMapper mapper)
        {
            _ = northwindDataAccessFactory ?? throw new ArgumentNullException(nameof(northwindDataAccessFactory));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            this.dataAccessObject = northwindDataAccessFactory.GetProductDataAccessObject();
        }

        /// <inheritdoc/>
        public async Task<int> CreateProductAsync(Product product)
        {
            _ = product ?? throw new ArgumentNullException(nameof(product));

            return await this.dataAccessObject.InsertProductAsync(this.mapper.Map<ProductTransferObject>(product));
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
            _ = names ?? throw new ArgumentNullException(nameof(names));

            var products = this.dataAccessObject.SelectProductsByNameAsync(names);
            await foreach (var product in products)
            {
                yield return this.mapper.Map<Product>(product);
            }
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<Product> GetProductsAsync(int offset, int limit)
        {
            var products = this.dataAccessObject.SelectProductsAsync(offset, limit);
            await foreach (var product in products)
            {
                yield return this.mapper.Map<Product>(product);
            }
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<Product> GetProductsForCategoryAsync(int categoryId)
        {
            var products = this.dataAccessObject.SelectProductByCategoryAsync(new[] { categoryId });
            await foreach (var product in products)
            {
                yield return this.mapper.Map<Product>(product);
            }
        }

        /// <inheritdoc/>
        public async Task<Product> GetProductAsync(int productId)
        {
            try
            {
                var productTransferObject = await this.dataAccessObject.FindProductAsync(productId);
                return this.mapper.Map<Product>(productTransferObject);
            }
            catch (ProductNotFoundException)
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateProductAsync(int productId, Product product)
        {
            _ = product ?? throw new ArgumentNullException(nameof(product));

            if (await this.dataAccessObject.UpdateProductAsync(productId, this.mapper.Map<ProductTransferObject>(product)))
            {
                return true;
            }

            return false;
        }
    }
}
