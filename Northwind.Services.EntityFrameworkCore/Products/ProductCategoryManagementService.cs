using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Northwind.Services.Products;
using Context = Northwind.Services.EntityFrameworkCore.Models;

namespace Northwind.Services.EntityFrameworkCore.Products
{
    public class ProductCategoryManagementService : IProductCategoryManagementService
    {
        private readonly string connectionString;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCategoryManagementService"/> class.
        /// </summary>
        /// <param name="mapper">Mapper for entity mapping.</param>
        public ProductCategoryManagementService(string connectionString, IMapper mapper)
        {
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<int> CreateCategoryAsync(ProductCategory productCategory)
        {
            _ = productCategory ?? throw new ArgumentNullException(nameof(productCategory));

            await using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);
            await db.Categories.AddAsync(this.mapper.Map<Context.Category>(productCategory));
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

                var products = db.Products.Where(product => product.Category == category);
                db.Products.RemoveRange(products);

                var orderDetails = products.SelectMany(
                    p => db.OrderDetails.Where(orderDet => orderDet.Product == p));
                db.OrderDetails.RemoveRange(orderDetails);

                await db.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async IAsyncEnumerable<ProductCategory> GetCategoriesByNameAsync(IEnumerable<string> names)
        {
            _ = names ?? throw new ArgumentNullException(nameof(names));

            await using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);

            var categories = from category in db.Categories
                             from name in names
                             where category.CategoryName == name
                             select this.mapper.Map<ProductCategory>(category);

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
                .Select(c => this.mapper.Map<ProductCategory>(c));

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

            return this.mapper.Map<ProductCategory>(contextCategory);
        }

        public async Task<bool> UpdateCategoryAsync(int categoryId, ProductCategory productCategory)
        {
            _ = productCategory ?? throw new ArgumentNullException(nameof(productCategory));

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
    }
}
