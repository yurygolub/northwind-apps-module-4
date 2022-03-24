using System.IO;
using System.Threading.Tasks;

namespace Northwind.Services.Products
{
    /// <summary>
    /// Represents a management service for product categories pictures.
    /// </summary>
    public interface IProductCategoryPicturesService
    {
        /// <summary>
        /// Gets a product category picture with specified identifier.
        /// </summary>
        /// <param name="categoryId">A product category identifier.</param>
        /// <returns>Returns a picture <see cref="Stream"/>.</returns>
        Task<Stream> GetProductCategoryPictureAsync(int categoryId);

        /// <summary>
        /// Deletes an existed product category picture.
        /// </summary>
        /// <param name="categoryId">A product category identifier.</param>
        /// <returns>True if a product category picture is destroyed; otherwise false.</returns>
        Task<bool> DeleteProductCategoryPictureAsync(int categoryId);

        /// <summary>
        /// Updates a product category picture.
        /// </summary>
        /// <param name="categoryId">A product category identifier.</param>
        /// <param name="stream">A <see cref="Stream"/>.</param>
        /// <returns>True if a product category picture is updated; otherwise false.</returns>
        Task<bool> UpdateProductCategoryPictureAsync(int categoryId, Stream stream);
    }
}
