using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Northwind.Services.Products;

namespace Northwind.Services.InMemory.Products
{
    public class ProductCategoryManagementService : IProductCategoryManagementService
    {
        private readonly NorthwindContext northwindContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCategoryManagementService"/> class.
        /// </summary>
        /// <param name="northwindContext">NorthwindContext.</param>
        /// <exception cref="ArgumentNullException">Thrown if northwindContext is null.</exception>
        public ProductCategoryManagementService(NorthwindContext northwindContext)
        {
            if (northwindContext is null)
            {
                throw new ArgumentNullException(nameof(northwindContext));
            }

            this.northwindContext = northwindContext;
        }

        public async Task<int> CreateCategoryAsync(ProductCategory productCategory)
        {
            if (productCategory is null)
            {
                throw new ArgumentNullException(nameof(productCategory));
            }

            await this.northwindContext.ProductCategories.AddAsync(MapProductCategory(productCategory));
            await this.northwindContext.SaveChangesAsync();
            return productCategory.Id;
        }

        public async Task<bool> DestroyCategoryAsync(int categoryId)
        {
            var category = await this.northwindContext.ProductCategories.FindAsync(categoryId);
            if (category != null)
            {
                this.northwindContext.ProductCategories.Remove(category);
                await this.northwindContext.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async IAsyncEnumerable<ProductCategory> GetCategoriesByNameAsync(IEnumerable<string> names)
        {
            if (names is null)
            {
                throw new ArgumentNullException(nameof(names));
            }

            var categories = from category in this.northwindContext.ProductCategories
                           from name in names
                           where category.Name == name
                           select MapProductCategory(category);

            await foreach (var category in categories.AsAsyncEnumerable())
            {
                yield return category;
            }
        }

        public async IAsyncEnumerable<ProductCategory> GetCategoriesAsync(int offset, int limit)
        {
            var categories = this.northwindContext.ProductCategories
                    .Skip(offset)
                    .Take(limit)
                    .Select(c => MapProductCategory(c));

            await foreach (var category in categories.AsAsyncEnumerable())
            {
                yield return category;
            }
        }

        public async Task<ProductCategory> GetCategoryAsync(int categoryId)
        {
            var contextCategory = await this.northwindContext.ProductCategories.FindAsync(categoryId);
            if (contextCategory is null)
            {
                return null;
            }

            return MapProductCategory(contextCategory);
        }

        public async Task<bool> UpdateCategoryAsync(int categoryId, ProductCategory productCategory)
        {
            if (productCategory is null)
            {
                throw new ArgumentNullException(nameof(productCategory));
            }

            var contextCategory = await this.northwindContext.ProductCategories.FindAsync(categoryId);
            if (contextCategory is null)
            {
                return false;
            }

            contextCategory.Name = productCategory.Name;
            contextCategory.Description = productCategory.Description;
            contextCategory.Picture = productCategory.Picture;

            await this.northwindContext.SaveChangesAsync();
            return true;
        }

        private static ProductCategory MapProductCategory(Entities.ProductCategory category)
        {
            return new ProductCategory()
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Picture = category.Picture,
            };
        }

        private static Entities.ProductCategory MapProductCategory(ProductCategory productCategory)
        {
            return new Entities.ProductCategory()
            {
                Id = productCategory.Id,
                Name = productCategory.Name,
                Description = productCategory.Description,
                Picture = productCategory.Picture,
            };
        }
    }
}
