using System;
using System.Collections.Generic;
using System.Linq;
using Northwind.Services.Products;

namespace Northwind.Services.Implementation.Products
{
    /// <summary>
    /// Represents a stub for a product management service.
    /// </summary>
    public sealed class ProductManagementService : IProductManagementService
    {
        /// <inheritdoc/>
        public int CreateProduct(Product product)
        {
            if (product is null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            NorthwindContext.Products.Add(product);
            return product.Id;
        }

        /// <inheritdoc/>
        public bool DestroyProduct(int productId)
        {
            int index = NorthwindContext.Products.FindIndex(p => p.Id == productId);
            if (index == -1)
            {
                return false;
            }

            NorthwindContext.Products.RemoveAt(index);
            return true;
        }

        /// <inheritdoc/>
        public IList<Product> LookupProductsByName(IList<string> names)
        {
            if (names is null)
            {
                throw new ArgumentNullException(nameof(names));
            }

            List<Product> products = new List<Product>();
            foreach (var name in names)
            {
                Product product = NorthwindContext.Products.Find(p => p.Name == name);
                if (product != null)
                {
                    products.Add(product);
                }
            }

            return products;
        }

        /// <inheritdoc/>
        public IList<Product> ShowProducts()
        {
            return NorthwindContext.Products;
        }

        /// <inheritdoc/>
        public IList<Product> ShowProductsForCategory(int categoryId)
        {
            return NorthwindContext.Products.Where(p => p.CategoryId == categoryId).ToList();
        }

        /// <inheritdoc/>
        public bool TryShowProduct(int productId, out Product product)
        {
            product = NorthwindContext.Products.Find(p => p.Id == productId);
            if (product is null)
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public bool UpdateProduct(int productId, Product product)
        {
            int index = NorthwindContext.Products.FindIndex(p => p.Id == productId);
            if (index == -1)
            {
                return false;
            }

            NorthwindContext.Products[index] = product;
            return true;
        }
    }
}
