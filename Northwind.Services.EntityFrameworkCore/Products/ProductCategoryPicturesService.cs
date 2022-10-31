using System;
using System.IO;
using System.Threading.Tasks;
using Northwind.Services.Products;

namespace Northwind.Services.EntityFrameworkCore.Products
{
    public class ProductCategoryPicturesService : IProductCategoryPicturesService
    {
        private readonly string connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCategoryManagementService"/> class.
        /// </summary>
        public ProductCategoryPicturesService(string connectionString)
        {
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<Stream> GetProductCategoryPictureAsync(int categoryId)
        {
            await using Models.NorthwindContext db = new Models.NorthwindContext(this.connectionString);

            var contextCategory = await db.Categories.FindAsync(categoryId);
            if (contextCategory?.Picture is null)
            {
                return null;
            }

            return new MemoryStream(contextCategory.Picture[78..]);
        }

        public async Task<bool> DeleteProductCategoryPictureAsync(int categoryId)
        {
            await using Models.NorthwindContext db = new Models.NorthwindContext(this.connectionString);

            var contextCategory = await db.Categories.FindAsync(categoryId);
            if (contextCategory is null)
            {
                return false;
            }

            contextCategory.Picture = null;

            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateProductCategoryPictureAsync(int categoryId, Stream stream)
        {
            _ = stream ?? throw new ArgumentNullException(nameof(stream));

            await using Models.NorthwindContext db = new Models.NorthwindContext(this.connectionString);

            var contextCategory = await db.Categories.FindAsync(categoryId);
            if (contextCategory is null)
            {
                return false;
            }

            await using MemoryStream memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.ToArray().CopyTo(contextCategory.Picture, 78);

            await db.SaveChangesAsync();
            return true;
        }
    }
}
