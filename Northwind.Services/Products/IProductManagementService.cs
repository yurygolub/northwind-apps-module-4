﻿using System.Collections.Generic;

namespace Northwind.Services.Products
{
    /// <summary>
    /// Represents a management service for products.
    /// </summary>
    public interface IProductManagementService
    {
        /// <summary>
        /// Shows a list of products using specified offset and limit for pagination.
        /// </summary>
        /// <param name="offset">An offset of the first element to return.</param>
        /// <param name="limit">A limit of elements to return.</param>
        /// <returns>A <see cref="IAsyncEnumerable{T}"/> of <see cref="Product"/>.</returns>
        IAsyncEnumerable<Product> GetProductsAsync(int offset, int limit);

        /// <summary>
        /// Try to show a product with specified identifier.
        /// </summary>
        /// <param name="productId">A product identifier.</param>
        /// <param name="product">A product to return.</param>
        /// <returns>Returns true if a product is returned; otherwise false.</returns>
        bool TryShowProduct(int productId, out Product product);

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="product">A <see cref="Product"/> to create.</param>
        /// <returns>An identifier of a created product.</returns>
        int CreateProduct(Product product);

        /// <summary>
        /// Destroys an existed product.
        /// </summary>
        /// <param name="productId">A product identifier.</param>
        /// <returns>True if a product is destroyed; otherwise false.</returns>
        bool DestroyProduct(int productId);

        /// <summary>
        /// Looks up for product with specified names.
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
        bool UpdateProduct(int productId, Product product);

        /// <summary>
        /// Shows a list of products that belongs to a specified category.
        /// </summary>
        /// <param name="categoryId">A product category identifier.</param>
        /// <returns>A <see cref="IAsyncEnumerable{T}"/> of <see cref="Product"/>.</returns>
        IAsyncEnumerable<Product> GetProductsForCategoryAsync(int categoryId);
    }
}
