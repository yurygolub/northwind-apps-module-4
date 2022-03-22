using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Northwind.Services.Products;
using Context = Northwind.Services.EntityFrameworkCore.Models;

namespace Northwind.Services.EntityFrameworkCore.Products
{
    public class ProductManagementService : IProductManagementService
    {
        private readonly string connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductManagementService"/> class.
        /// </summary>
        public ProductManagementService(string connectionString)
        {
            if (connectionString is null)
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            this.connectionString = connectionString;
        }

        public int CreateProduct(Product product)
        {
            if (product is null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);
            db.Products.Add(MapProduct(product));
            db.SaveChanges();
            return product.Id;
        }

        public bool DestroyProduct(int productId)
        {
            using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);

            var product = db.Products.Find(productId);
            if (product != null)
            {
                db.Products.Remove(product);
                db.SaveChanges();
                return true;
            }

            return false;
        }

        public async IAsyncEnumerable<Product> GetProductsByNameAsync(IEnumerable<string> names)
        {
            if (names is null)
            {
                throw new ArgumentNullException(nameof(names));
            }

            await using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);
            var products = await Task.Run(() => GetProductsByName(db, names));
            foreach (var product in products)
            {
                yield return product;
            }

            static IEnumerable<Product> GetProductsByName(Context.NorthwindContext db, IEnumerable<string> names)
            {
                return from product in db.Products
                       from name in names
                       where product.ProductName == name
                       select MapProduct(product);
            }
        }

        public async IAsyncEnumerable<Product> GetProductsAsync(int offset, int limit)
        {
            await using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);

            var products = await Task.Run(() => GetProducts(db, offset, limit));
            foreach (var product in products)
            {
                yield return product;
            }

            static IEnumerable<Product> GetProducts(Context.NorthwindContext db, int offset, int limit)
            {
                return db.Products
                    .Skip(offset)
                    .Take(limit)
                    .Select(p => MapProduct(p));
            }
        }

        public async IAsyncEnumerable<Product> GetProductsForCategoryAsync(int categoryId)
        {
            await using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);

            var products = await Task.Run(() => GetProductsForCategory(db, categoryId));
            foreach (var product in products)
            {
                yield return product;
            }

            static IEnumerable<Product> GetProductsForCategory(Context.NorthwindContext db, int categoryId)
            {
                return db.Products
                    .Where(p => p.CategoryId == categoryId)
                    .Select(p => MapProduct(p))
                    .ToList();
            }
        }

        public bool TryShowProduct(int productId, out Product product)
        {
            using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);

            var contextProduct = db.Products.Find(productId);
            product = null;
            if (contextProduct is null)
            {
                return false;
            }

            product = MapProduct(contextProduct);
            return true;
        }

        public bool UpdateProduct(int productId, Product product)
        {
            if (product is null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);

            var contextProduct = db.Products.Find(productId);
            if (contextProduct is null)
            {
                return false;
            }

            contextProduct = MapProduct(product);
            db.SaveChanges();
            return true;
        }

        private static Product MapProduct(Context.Product product)
        {
            return new Product()
            {
                Id = product.ProductId,
                CategoryId = product.CategoryId,
                Discontinued = product.Discontinued,
                Name = product.ProductName,
                QuantityPerUnit = product.QuantityPerUnit,
                ReorderLevel = product.ReorderLevel,
                SupplierId = product.SupplierId,
                UnitPrice = product.UnitPrice,
                UnitsInStock = product.UnitsInStock,
                UnitsOnOrder = product.UnitsOnOrder,
            };
        }

        private static Context.Product MapProduct(Product product)
        {
            return new Context.Product()
            {
                ProductId = product.Id,
                CategoryId = product.CategoryId,
                Discontinued = product.Discontinued,
                ProductName = product.Name,
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
