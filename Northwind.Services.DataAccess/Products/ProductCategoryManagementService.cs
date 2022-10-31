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
    /// Represents a stub for a product category management service.
    /// </summary>
    public sealed class ProductCategoryManagementService : IProductCategoryManagementService
    {
        private readonly IProductCategoryDataAccessObject dataAccessObject;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCategoryManagementService"/> class.
        /// </summary>
        /// <param name="northwindDataAccessFactory">Factory for creating Northwind DAO.</param>
        /// <param name="mapper">Mapper for entity mapping.</param>
        public ProductCategoryManagementService(NorthwindDataAccessFactory northwindDataAccessFactory, IMapper mapper)
        {
            if (northwindDataAccessFactory is null)
            {
                throw new ArgumentNullException(nameof(northwindDataAccessFactory));
            }

            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            this.dataAccessObject = northwindDataAccessFactory.GetProductCategoryDataAccessObject();
        }

        /// <inheritdoc/>
        public async Task<int> CreateCategoryAsync(ProductCategory productCategory)
        {
            if (productCategory is null)
            {
                throw new ArgumentNullException(nameof(productCategory));
            }

            return await this.dataAccessObject.InsertProductCategoryAsync(this.mapper.Map<ProductCategoryTransferObject>(productCategory));
        }

        /// <inheritdoc/>
        public async Task<bool> DestroyCategoryAsync(int categoryId)
        {
            if (await this.dataAccessObject.DeleteProductCategoryAsync(categoryId))
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ProductCategory> GetCategoriesByNameAsync(IEnumerable<string> names)
        {
            if (names is null)
            {
                throw new ArgumentNullException(nameof(names));
            }

            var productCategories = this.dataAccessObject.SelectProductCategoriesByNameAsync(names);
            await foreach (var productCategory in productCategories)
            {
                yield return this.mapper.Map<ProductCategory>(productCategory);
            }
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ProductCategory> GetCategoriesAsync(int offset, int limit)
        {
            var productCategories = this.dataAccessObject.SelectProductCategoriesAsync(offset, limit);
            await foreach (var productCategory in productCategories)
            {
                yield return this.mapper.Map<ProductCategory>(productCategory);
            }
        }

        /// <inheritdoc/>
        public async Task<ProductCategory> GetCategoryAsync(int categoryId)
        {
            try
            {
                var productTransferObject = await this.dataAccessObject.FindProductCategoryAsync(categoryId);
                return this.mapper.Map<ProductCategory>(productTransferObject);
            }
            catch (ProductCategoryNotFoundException)
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateCategoryAsync(int categoryId, ProductCategory productCategory)
        {
            if (productCategory is null)
            {
                throw new ArgumentNullException(nameof(productCategory));
            }

            if (await this.dataAccessObject.UpdateProductCategoryAsync(categoryId, this.mapper.Map<ProductCategoryTransferObject>(productCategory)))
            {
                return true;
            }

            return false;
        }
    }
}
