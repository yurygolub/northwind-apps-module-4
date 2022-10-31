using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Northwind.Services.Products;

namespace Northwind.Services.InMemory.Products
{
    public class ProductCategoryManagementService : IProductCategoryManagementService
    {
        private readonly NorthwindContext northwindContext;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCategoryManagementService"/> class.
        /// </summary>
        /// <param name="northwindContext">NorthwindContext.</param>
        /// <param name="mapper">Mapper for entity mapping.</param>
        /// <exception cref="ArgumentNullException">Thrown if northwindContext is null.</exception>
        public ProductCategoryManagementService(NorthwindContext northwindContext, IMapper mapper)
        {
            this.northwindContext = northwindContext ?? throw new ArgumentNullException(nameof(northwindContext));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<int> CreateCategoryAsync(ProductCategory productCategory)
        {
            _ = productCategory ?? throw new ArgumentNullException(nameof(productCategory));

            await this.northwindContext.ProductCategories.AddAsync(this.mapper.Map<Entities.ProductCategory>(productCategory));
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
            _ = names ?? throw new ArgumentNullException(nameof(names));

            var categories = from category in this.northwindContext.ProductCategories
                             from name in names
                             where category.Name == name
                             select this.mapper.Map<ProductCategory>(category);

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
                .Select(c => this.mapper.Map<ProductCategory>(c));

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

            return this.mapper.Map<ProductCategory>(contextCategory);
        }

        public async Task<bool> UpdateCategoryAsync(int categoryId, ProductCategory productCategory)
        {
            _ = productCategory ?? throw new ArgumentNullException(nameof(productCategory));

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
    }
}
