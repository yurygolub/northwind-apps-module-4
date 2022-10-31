using System;
using System.IO;
using System.Threading.Tasks;
using Northwind.DataAccess;
using Northwind.DataAccess.Products;
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
            _ = northwindDataAccessFactory ?? throw new ArgumentNullException(nameof(northwindDataAccessFactory));

            this.dataAccessObject = northwindDataAccessFactory.GetProductCategoryDataAccessObject();
        }

        public async Task<Stream> GetProductCategoryPictureAsync(int categoryId)
        {
            try
            {
                var category = await this.dataAccessObject.FindProductCategoryAsync(categoryId);
                if (category.Picture is null)
                {
                    return null;
                }

                return new MemoryStream(category.Picture[78..]);
            }
            catch (ProductCategoryNotFoundException)
            {
                return null;
            }
        }

        public async Task<bool> DeleteProductCategoryPictureAsync(int categoryId)
        {
            ProductCategoryTransferObject category;
            try
            {
                category = await this.dataAccessObject.FindProductCategoryAsync(categoryId);
            }
            catch (ProductCategoryNotFoundException)
            {
                return false;
            }

            category.Picture = null;

            return await this.dataAccessObject.UpdateProductCategoryAsync(categoryId, category);
        }

        public async Task<bool> UpdateProductCategoryPictureAsync(int categoryId, Stream stream)
        {
            _ = stream ?? throw new ArgumentNullException(nameof(stream));

            var category = await this.dataAccessObject.FindProductCategoryAsync(categoryId);
            if (category is null)
            {
                return false;
            }

            await using MemoryStream memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.ToArray().CopyTo(category.Picture, 78);

            return await this.dataAccessObject.UpdateProductCategoryAsync(categoryId, category);
        }
    }
}
