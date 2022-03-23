﻿using System;
using System.Collections.Generic;
using Northwind.Services.Products;

#pragma warning disable S4457

namespace Northwind.Services.DataAccess.Products
{
    /// <summary>
    /// Represents a stub for a product management service.
    /// </summary>
    public sealed class ProductManagementService : IProductManagementService
    {
        private readonly IProductDataAccessObject dataAccessObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductManagementService"/> class.
        /// </summary>
        /// <param name="northwindDataAccessFactory">Factory for creating Northwind DAO.</param>
        public ProductManagementService(NorthwindDataAccessFactory northwindDataAccessFactory)
        {
            if (northwindDataAccessFactory is null)
            {
                throw new ArgumentNullException(nameof(northwindDataAccessFactory));
            }

            this.dataAccessObject = northwindDataAccessFactory.GetProductDataAccessObject();
        }

        /// <inheritdoc/>
        public int CreateProduct(Product product)
        {
            if (product is null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            return this.dataAccessObject.InsertProduct(MapProduct(product));
        }

        /// <inheritdoc/>
        public bool DestroyProduct(int productId)
        {
            if (this.dataAccessObject.DeleteProduct(productId))
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<Product> GetProductsByNameAsync(IEnumerable<string> names)
        {
            if (names is null)
            {
                throw new ArgumentNullException(nameof(names));
            }

            var products = this.dataAccessObject.SelectProductsByName(names);
            await foreach (var product in products)
            {
                yield return MapProduct(product);
            }
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<Product> GetProductsAsync(int offset, int limit)
        {
            var products = this.dataAccessObject.SelectProducts(offset, limit);
            await foreach (var product in products)
            {
                yield return MapProduct(product);
            }
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<Product> GetProductsForCategoryAsync(int categoryId)
        {
            var products = this.dataAccessObject.SelectProductByCategory(new[] { categoryId });
            await foreach (var product in products)
            {
                yield return MapProduct(product);
            }
        }

        /// <inheritdoc/>
        public bool TryShowProduct(int productId, out Product product)
        {
            var productTransferObject = this.dataAccessObject.FindProduct(productId);
            product = MapProduct(productTransferObject);
            if (product is null)
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public bool UpdateProduct(int productId, Product product)
        {
            if (this.dataAccessObject.UpdateProduct(MapProduct(product)))
            {
                return true;
            }

            return false;
        }

        private static Product MapProduct(ProductTransferObject productTransferObject)
        {
            if (productTransferObject is null)
            {
                return null;
            }

            return new Product()
            {
                Id = productTransferObject.Id,
                CategoryId = productTransferObject.CategoryId,
                Discontinued = productTransferObject.Discontinued,
                Name = productTransferObject.Name,
                QuantityPerUnit = productTransferObject.QuantityPerUnit,
                ReorderLevel = productTransferObject.ReorderLevel,
                SupplierId = productTransferObject.SupplierId,
                UnitPrice = productTransferObject.UnitPrice,
                UnitsInStock = productTransferObject.UnitsInStock,
                UnitsOnOrder = productTransferObject.UnitsOnOrder,
            };
        }

        private static ProductTransferObject MapProduct(Product product)
        {
            if (product is null)
            {
                return null;
            }

            return new ProductTransferObject()
            {
                Id = product.Id,
                CategoryId = product.CategoryId,
                Discontinued = product.Discontinued,
                Name = product.Name,
                QuantityPerUnit = product.QuantityPerUnit,
                ReorderLevel = product.ReorderLevel,
                SupplierId = product.SupplierId,
                UnitPrice = product.UnitPrice,
                UnitsInStock = product.UnitsInStock,
                UnitsOnOrder = product.UnitsOnOrder,
            };
        }
    }
}