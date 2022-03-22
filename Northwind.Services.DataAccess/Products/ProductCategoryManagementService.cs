using System;
using System.Collections.Generic;
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
        public int CreateCategory(ProductCategory productCategory)
        {
            if (productCategory is null)
            {
                throw new ArgumentNullException(nameof(productCategory));
            }

            return this.dataAccessObject.InsertProductCategory(MapProductCategory(productCategory));
        }

        /// <inheritdoc/>
        public bool DestroyCategory(int categoryId)
        {
            if (this.dataAccessObject.DeleteProductCategory(categoryId))
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
        public bool TryShowCategory(int categoryId, out ProductCategory productCategory)
        {
            var productTransferObject = this.dataAccessObject.FindProductCategory(categoryId);
            productCategory = MapProductCategory(productTransferObject);
            if (productCategory is null)
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public bool UpdateCategory(int categoryId, ProductCategory productCategory)
        {
            if (this.dataAccessObject.UpdateProductCategory(MapProductCategory(productCategory)))
            {
                return true;
            }

            return false;
        }

        private static ProductCategory MapProductCategory(ProductCategoryTransferObject productCategoruTransferObject)
        {
            if (productCategoruTransferObject is null)
            {
                return null;
            }

            return new ProductCategory()
            {
                Id = productCategoruTransferObject.Id,
                Name = productCategoruTransferObject.Name,
                Description = productCategoruTransferObject.Description,
            };
        }

        private static ProductCategoryTransferObject MapProductCategory(ProductCategory productCategory)
        {
            if (productCategory is null)
            {
                return null;
            }

            return new ProductCategoryTransferObject()
            {
                Id = productCategory.Id,
                Name = productCategory.Name,
                Description = productCategory.Description,
            };
        }
    }
}
