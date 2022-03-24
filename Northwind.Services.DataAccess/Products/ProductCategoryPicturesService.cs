using System;
using System.IO;
using System.Threading.Tasks;
using Northwind.Services.Products;

namespace Northwind.Services.DataAccess.Products
{
    public class ProductCategoryPicturesService : IProductCategoryPicturesService
    {
        private readonly IProductCategoryDataAccessObject dataAccessObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCategoryPicturesService"/> class.
        /// </summary>
        /// <param name="northwindDataAccessFactory">Factory for creating Northwind DAO.</param>
        public ProductCategoryPicturesService(NorthwindDataAccessFactory northwindDataAccessFactory)
        {
            if (northwindDataAccessFactory is null)
            {
                throw new ArgumentNullException(nameof(northwindDataAccessFactory));
            }

            this.dataAccessObject = northwindDataAccessFactory.GetProductCategoryDataAccessObject();
        }

        public async Task<Stream> GetProductCategoryPictureAsync(int categoryId)
        {
            var category = await this.dataAccessObject.FindProductCategoryAsync(categoryId);
            if (category?.Picture is null)
            {
                return null;
            }

            return new MemoryStream(category.Picture[78..]);
        }

        public async Task<bool> DeleteProductCategoryPictureAsync(int categoryId)
        {
            var category = await this.dataAccessObject.FindProductCategoryAsync(categoryId);
            if (category is null)
            {
                return false;
            }

            category.Picture = null;

            if (await this.dataAccessObject.UpdateProductCategoryAsync(categoryId, category))
            {
                return true;
            }

            return true;
        }

        public async Task<bool> UpdateProductCategoryPictureAsync(int categoryId, Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var category = await this.dataAccessObject.FindProductCategoryAsync(categoryId);
            if (category is null)
            {
                return false;
            }

            await using MemoryStream memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.ToArray().CopyTo(category.Picture, 78);

            if (await this.dataAccessObject.UpdateProductCategoryAsync(categoryId, category))
            {
                return true;
            }

            return true;
        }
    }
}
