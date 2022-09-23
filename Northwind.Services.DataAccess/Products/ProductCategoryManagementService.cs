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
    /// Represents a stub for a product category management service.
    /// </summary>
    public sealed class ProductCategoryManagementService : IProductCategoryManagementService
    {
        private readonly IProductCategoryDataAccessObject dataAccessObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCategoryManagementService"/> class.
        /// </summary>
        /// <param name="northwindDataAccessFactory">Factory for creating Northwind DAO.</param>
        public ProductCategoryManagementService(NorthwindDataAccessFactory northwindDataAccessFactory)
        {
            if (northwindDataAccessFactory is null)
            {
                throw new ArgumentNullException(nameof(northwindDataAccessFactory));
            }

            this.dataAccessObject = northwindDataAccessFactory.GetProductCategoryDataAccessObject();
        }

        /// <inheritdoc/>
        public async Task<int> CreateCategoryAsync(ProductCategory productCategory)
        {
            if (productCategory is null)
            {
                throw new ArgumentNullException(nameof(productCategory));
            }

            return await this.dataAccessObject.InsertProductCategoryAsync(MapProductCategory(productCategory));
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
                yield return MapProductCategory(productCategory);
            }
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ProductCategory> GetCategoriesAsync(int offset, int limit)
        {
            var productCategories = this.dataAccessObject.SelectProductCategoriesAsync(offset, limit);
            await foreach (var productCategory in productCategories)
            {
                yield return MapProductCategory(productCategory);
            }
        }

        /// <inheritdoc/>
        public async Task<ProductCategory> GetCategoryAsync(int categoryId)
        {
            try
            {
                var productTransferObject = await this.dataAccessObject.FindProductCategoryAsync(categoryId);
                return MapProductCategory(productTransferObject);
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

            if (await this.dataAccessObject.UpdateProductCategoryAsync(categoryId, MapProductCategory(productCategory)))
            {
                return true;
            }

            return false;
        }

        private static ProductCategory MapProductCategory(ProductCategoryTransferObject productCategoruTransferObject)
        {
            return new ProductCategory()
            {
                Id = productCategoruTransferObject.Id,
                Name = productCategoruTransferObject.Name,
                Description = productCategoruTransferObject.Description,
                Picture = productCategoruTransferObject.Picture,
            };
        }

        private static ProductCategoryTransferObject MapProductCategory(ProductCategory productCategory)
        {
            return new ProductCategoryTransferObject()
            {
                Id = productCategory.Id,
                Name = productCategory.Name,
                Description = productCategory.Description,
                Picture = productCategory.Picture,
            };
        }
    }
}
