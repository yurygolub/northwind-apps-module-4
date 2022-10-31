using System.Collections.Generic;
using System.Threading.Tasks;

namespace Northwind.Services.Products
{
    /// <summary>
    /// Represents a management service for products.
    /// </summary>
    public interface IProductManagementService
    {
        /// <summary>
        /// Gets a list of products using specified offset and limit for pagination.
        /// </summary>
        /// <param name="offset">An offset of the first element to return.</param>
        /// <param name="limit">A limit of elements to return.</param>
        /// <returns>A <see cref="IAsyncEnumerable{T}"/> of <see cref="Product"/>.</returns>
        IAsyncEnumerable<Product> GetProductsAsync(int offset, int limit);

        /// <summary>
        /// Gets a product with specified identifier.
        /// </summary>
        /// <param name="productId">A product identifier.</param>
        /// <returns>Returns a <see cref="Product"/> object.</returns>
        Task<Product> GetProductAsync(int productId);

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="product">A <see cref="Product"/> to create.</param>
        /// <returns>An identifier of a created product.</returns>
        Task<int> CreateProductAsync(Product product);

        /// <summary>
        /// Deletes an existed product.
        /// </summary>
        /// <param name="productId">A product identifier.</param>
        /// <returns>True if a product is destroyed; otherwise false.</returns>
        Task<bool> DeleteProductAsync(int productId);

        /// <summary>
        /// Gets product with specified names.
        /// </summary>
        /// <param name="names">A list of product names.</param>
        /// <returns>A list of products with specified names.</returns>
        IAsyncEnumerable<Product> GetProductsByNameAsync(IEnumerable<string> names);

        /// <summary>
        /// Updates a product.
        /// </summary>
        /// <param name="productId">A product identifier.</param>
        /// <param name="product">A <see cref="Product"/>.</param>
        /// <returns>True if a product is updated; otherwise false.</returns>
        Task<bool> UpdateProductAsync(int productId, Product product);

        /// <summary>
        /// Gets a list of products that belongs to a specified category.
        /// </summary>
        /// <param name="categoryId">A product category identifier.</param>
        /// <returns>A <see cref="IAsyncEnumerable{T}"/> of <see cref="Product"/>.</returns>
        IAsyncEnumerable<Product> GetProductsForCategoryAsync(int categoryId);
    }
}
