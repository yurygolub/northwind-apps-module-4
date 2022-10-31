using System;
using System.IO;
using System.Threading.Tasks;
using Northwind.Services.Products;

namespace Northwind.Services.InMemory.Products
{
    public class ProductCategoryPicturesService : IProductCategoryPicturesService
    {
        private readonly NorthwindContext northwindContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCategoryPicturesService"/> class.
        /// </summary>
        /// <param name="northwindContext">NorthwindContext.</param>
        /// <exception cref="ArgumentNullException">Thrown if northwindContext is null.</exception>
        public ProductCategoryPicturesService(NorthwindContext northwindContext)
        {
            this.northwindContext = northwindContext ?? throw new ArgumentNullException(nameof(northwindContext));
        }

        public async Task<Stream> GetProductCategoryPictureAsync(int categoryId)
        {
            var contextCategory = await this.northwindContext.ProductCategories.FindAsync(categoryId);
            if (contextCategory?.Picture is null)
            {
                return null;
            }

            return new MemoryStream(contextCategory.Picture[78..]);
        }

        public async Task<bool> DeleteProductCategoryPictureAsync(int categoryId)
        {
            var contextCategory = await this.northwindContext.ProductCategories.FindAsync(categoryId);
            if (contextCategory is null)
            {
                return false;
            }

            contextCategory.Picture = null;

            await this.northwindContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateProductCategoryPictureAsync(int categoryId, Stream stream)
        {
            _ = stream ?? throw new ArgumentNullException(nameof(stream));

            var contextCategory = await this.northwindContext.ProductCategories.FindAsync(categoryId);
            if (contextCategory is null)
            {
                return false;
            }

            await using MemoryStream memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.ToArray().CopyTo(contextCategory.Picture, 78);

            await this.northwindContext.SaveChangesAsync();
            return true;
        }
    }
}
