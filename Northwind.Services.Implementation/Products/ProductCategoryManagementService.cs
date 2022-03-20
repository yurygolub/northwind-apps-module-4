using System;
using System.Collections.Generic;
using Northwind.Services.Products;

namespace Northwind.Services.Implementation.Products
{
    /// <summary>
    /// Represents a stub for a product category management service.
    /// </summary>
    public sealed class ProductCategoryManagementService : IProductCategoryManagementService
    {
        /// <inheritdoc/>
        public int CreateCategory(ProductCategory productCategory)
        {
            if (productCategory is null)
            {
                throw new ArgumentNullException(nameof(productCategory));
            }

            NorthwindContext.ProductCategories.Add(productCategory);
            return productCategory.Id;
        }

        /// <inheritdoc/>
        public bool DestroyCategory(int categoryId)
        {
            int index = NorthwindContext.ProductCategories.FindIndex(p => p.Id == categoryId);
            if (index == -1)
            {
                return false;
            }

            NorthwindContext.ProductCategories.RemoveAt(index);
            return true;
        }

        /// <inheritdoc/>
        public IList<ProductCategory> LookupCategoriesByName(IList<string> names)
        {
            if (names is null)
            {
                throw new ArgumentNullException(nameof(names));
            }

            List<ProductCategory> producrCategories = new List<ProductCategory>();
            foreach (var name in names)
            {
                ProductCategory producrCategory = NorthwindContext.ProductCategories.Find(p => p.Name == name);
                if (producrCategory != null)
                {
                    producrCategories.Add(producrCategory);
                }
            }

            return producrCategories;
        }

        /// <inheritdoc/>
        public IList<ProductCategory> ShowCategories()
        {
            return NorthwindContext.ProductCategories;
        }

        /// <inheritdoc/>
        public bool TryShowCategory(int categoryId, out ProductCategory productCategory)
        {
            productCategory = NorthwindContext.ProductCategories.Find(p => p.Id == categoryId);
            if (productCategory is null)
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public bool UpdateCategory(int categoryId, ProductCategory productCategory)
        {
            int index = NorthwindContext.ProductCategories.FindIndex(p => p.Id == categoryId);
            if (index == -1)
            {
                return false;
            }

            NorthwindContext.ProductCategories[index] = productCategory;
            return true;
        }
    }
}
