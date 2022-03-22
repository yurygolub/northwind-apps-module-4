using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public int CreateCategory(ProductCategory productCategory)
        {
            if (productCategory is null)
            {
                throw new ArgumentNullException(nameof(productCategory));
            }

            using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);
            db.Categories.Add(MapProductCategory(productCategory));
            db.SaveChanges();
            return productCategory.Id;
        }

        public bool DestroyCategory(int categoryId)
        {
            using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);

            var category = db.Categories.Find(categoryId);
            if (category != null)
            {
                db.Categories.Remove(category);
                db.SaveChanges();
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

            var categories = await Task.Run(() => GetCategoriesByName(db, names));
            foreach (var category in categories)
            {
                yield return category;
            }

            static IEnumerable<ProductCategory> GetCategoriesByName(Context.NorthwindContext db, IEnumerable<string> names)
            {
                return from category in db.Categories
                        from name in names
                        where category.CategoryName == name
                        select MapProductCategory(category);
            }
        }

        public async IAsyncEnumerable<ProductCategory> GetCategoriesAsync(int offset, int limit)
        {
            await using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);

            var categories = await Task.Run(() => ShowCategories(db, offset, limit));
            foreach (var category in categories)
            {
                yield return category;
            }

            static IEnumerable<ProductCategory> ShowCategories(Context.NorthwindContext db, int offset, int limit)
            {
                return db.Categories
                    .Skip(offset)
                    .Take(limit)
                    .Select(c => MapProductCategory(c));
            }
        }

        public bool TryShowCategory(int categoryId, out ProductCategory productCategory)
        {
            using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);

            var contextCategory = db.Categories.Find(categoryId);
            productCategory = null;
            if (contextCategory is null)
            {
                return false;
            }

            productCategory = MapProductCategory(contextCategory);
            return true;
        }

        public bool UpdateCategory(int categoryId, ProductCategory productCategory)
        {
            if (productCategory is null)
            {
                throw new ArgumentNullException(nameof(productCategory));
            }

            using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);

            var contextCategory = db.Categories.Find(categoryId);
            if (contextCategory is null)
            {
                return false;
            }

            contextCategory = MapProductCategory(productCategory);
            db.SaveChanges();
            return true;
        }

        private static ProductCategory MapProductCategory(Context.Category category)
        {
            return new ProductCategory()
            {
                Id = category.CategoryId,
                Name = category.CategoryName,
                Description = category.Description,
            };
        }

        private static Context.Category MapProductCategory(ProductCategory productCategory)
        {
            return new Context.Category()
            {
                CategoryId = productCategory.Id,
                CategoryName = productCategory.Name,
                Description = productCategory.Description,
            };
        }
    }
}
