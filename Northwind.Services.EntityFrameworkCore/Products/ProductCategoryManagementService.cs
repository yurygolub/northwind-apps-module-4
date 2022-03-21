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

        public async Task<IList<ProductCategory>> LookupCategoriesByName(IList<string> names)
        {
            if (names is null)
            {
                throw new ArgumentNullException(nameof(names));
            }

            using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);

            return await Task.Run(() => LookupCategoriesByName(db, names));

            static IList<ProductCategory> LookupCategoriesByName(Context.NorthwindContext db, IList<string> names)
            {
                return (from category in db.Categories
                        from name in names
                        where category.CategoryName == name
                        select MapProductCategory(category))
                            .ToList();
            }
        }

        public async Task<IList<ProductCategory>> ShowCategoriesAsync(int offset, int limit)
        {
            using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);

            return await Task.Run(() => ShowCategories(db, offset, limit));

            static IList<ProductCategory> ShowCategories(Context.NorthwindContext db, int offset, int limit)
            {
                return db.Categories
                    .Skip(offset)
                    .Take(limit)
                    .Select(c => MapProductCategory(c))
                    .ToList();
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
