using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Northwind.Services.Products;
using Context = Northwind.Services.EntityFrameworkCore.Models;

namespace Northwind.Services.EntityFrameworkCore.Products
{
    public class ProductCategoryManagementService : IProductCategoryManagementService
    {
        private readonly string connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCategoryManagementService"/> class.
        /// </summary>
        public ProductCategoryManagementService(string connectionString)
        {
            if (connectionString is null)
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            this.connectionString = connectionString;
        }

        public async Task<int> CreateCategoryAsync(ProductCategory productCategory)
        {
            if (productCategory is null)
            {
                throw new ArgumentNullException(nameof(productCategory));
            }

            await using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);
            await db.Categories.AddAsync(MapProductCategory(productCategory));
            await db.SaveChangesAsync();
            return productCategory.Id;
        }

        public async Task<bool> DestroyCategoryAsync(int categoryId)
        {
            await using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);

            var category = await db.Categories.FindAsync(categoryId);
            if (category != null)
            {
                db.Categories.Remove(category);
                await db.SaveChangesAsync();
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

            await using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);

            var categories = from category in db.Categories
                           from name in names
                           where category.CategoryName == name
                           select MapProductCategory(category);

            await foreach (var category in categories.AsAsyncEnumerable())
            {
                yield return category;
            }
        }

        public async IAsyncEnumerable<ProductCategory> GetCategoriesAsync(int offset, int limit)
        {
            await using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);

            var categories = db.Categories
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
            await using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);

            var contextCategory = await db.Categories.FindAsync(categoryId);
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

            await using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);

            var contextCategory = await db.Categories.FindAsync(categoryId);
            if (contextCategory is null)
            {
                return false;
            }

            contextCategory.CategoryName = productCategory.Name;
            contextCategory.Description = productCategory.Description;
            contextCategory.Picture = productCategory.Picture;

            await db.SaveChangesAsync();
            return true;
        }

        private static ProductCategory MapProductCategory(Context.Category category)
        {
            return new ProductCategory()
            {
                Id = category.CategoryId,
                Name = category.CategoryName,
                Description = category.Description,
                Picture = category.Picture,
            };
        }

        private static Context.Category MapProductCategory(ProductCategory productCategory)
        {
            return new Context.Category()
            {
                CategoryId = productCategory.Id,
                CategoryName = productCategory.Name,
                Description = productCategory.Description,
                Picture = productCategory.Picture,
            };
        }
    }
}
