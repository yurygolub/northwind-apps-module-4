using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Northwind.Services.Products;

namespace Northwind.Services.EntityFrameworkCore.Products
{
    public class ProductCategoryManagementService : IProductCategoryManagementService
    {
        private readonly Models.NorthwindContext context;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCategoryManagementService"/> class.
        /// </summary>
        /// <param name="mapper">Mapper for entity mapping.</param>
        public ProductCategoryManagementService(Models.NorthwindContext context, IMapper mapper)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<int> CreateCategoryAsync(ProductCategory productCategory)
        {
            _ = productCategory ?? throw new ArgumentNullException(nameof(productCategory));

            await this.context.Categories.AddAsync(this.mapper.Map<Models.Category>(productCategory));
            await this.context.SaveChangesAsync();
            return productCategory.Id;
        }

        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            var category = await this.context.Categories.FindAsync(categoryId);
            if (category != null)
            {
                this.context.Categories.Remove(category);

                var products = this.context.Products.Where(product => product.Category == category);
                this.context.Products.RemoveRange(products);

                var orderDetails = products.SelectMany(
                    p => this.context.OrderDetails.Where(orderDet => orderDet.Product == p));
                this.context.OrderDetails.RemoveRange(orderDetails);

                await this.context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async IAsyncEnumerable<ProductCategory> GetCategoriesByNameAsync(IEnumerable<string> names)
        {
            _ = names ?? throw new ArgumentNullException(nameof(names));

            var categories = from category in this.context.Categories
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
            var categories = this.context.Categories
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
            var contextCategory = await this.context.Categories.FindAsync(categoryId);
            if (contextCategory is null)
            {
                return null;
            }

            return this.mapper.Map<ProductCategory>(contextCategory);
        }

        public async Task<bool> UpdateCategoryAsync(int categoryId, ProductCategory productCategory)
        {
            _ = productCategory ?? throw new ArgumentNullException(nameof(productCategory));

            var contextCategory = await this.context.Categories.FindAsync(categoryId);
            if (contextCategory is null)
            {
                return false;
            }

            contextCategory.CategoryName = productCategory.Name;
            contextCategory.Description = productCategory.Description;
            contextCategory.Picture = productCategory.Picture;

            await this.context.SaveChangesAsync();
            return true;
        }
    }
}
