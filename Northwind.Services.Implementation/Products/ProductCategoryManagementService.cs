using System;
using System.Collections.Generic;
using System.Linq;
using Northwind.DataAccess;
using Northwind.DataAccess.Products;
using Northwind.Services.Products;

namespace Northwind.Services.Implementation.Products
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
        public IList<ProductCategory> LookupCategoriesByName(IList<string> names)
        {
            if (names is null)
            {
                throw new ArgumentNullException(nameof(names));
            }

            return this.dataAccessObject
                .SelectProductCategoriesByName(names)
                .Select(p => MapProductCategory(p))
                .ToList();
        }

        /// <inheritdoc/>
        public IList<ProductCategory> ShowCategories(int offset, int limit)
        {
            return this.dataAccessObject
                .SelectProductCategories(offset, limit)
                .Select(p => MapProductCategory(p))
                .ToList();
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
