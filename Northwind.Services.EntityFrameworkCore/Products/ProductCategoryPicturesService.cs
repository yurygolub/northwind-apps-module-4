using System;
using System.IO;
using System.Threading.Tasks;
using Northwind.Services.Products;
using Context = Northwind.Services.EntityFrameworkCore.Models;

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
            if (connectionString is null)
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            this.connectionString = connectionString;
        }

        public async Task<Stream> GetProductCategoryPictureAsync(int categoryId)
        {
            await using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);

            var contextCategory = await db.Categories.FindAsync(categoryId);
            if (contextCategory?.Picture is null)
            {
                return null;
            }

            return new MemoryStream(contextCategory.Picture[78..]);
        }

        public async Task<bool> DeleteProductCategoryPictureAsync(int categoryId)
        {
            await using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);

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
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            await using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);

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
