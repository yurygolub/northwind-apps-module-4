using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Northwind.Services.Products;

#pragma warning disable S4457

namespace Northwind.Services.SqlServer.Products
{
    /// <summary>
    /// Represents a SQL Server-tailored DAO for Northwind products.
    /// </summary>
    public sealed class ProductSqlServerDataAccessObject : IProductDataAccessObject
    {
        private readonly SqlConnection connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductSqlServerDataAccessObject"/> class.
        /// </summary>
        /// <param name="connection">A <see cref="SqlConnection"/>.</param>
        public ProductSqlServerDataAccessObject(SqlConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        /// <inheritdoc/>
        public async Task<int> InsertProductAsync(ProductTransferObject product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            await using var command = new SqlCommand("InsertProduct", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            AddSqlParameters(product, command);

            if (this.connection.State != ConnectionState.Open)
            {
                await this.connection.OpenAsync();
            }

            return await command.ExecuteNonQueryAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteProductAsync(int productId)
        {
            if (productId <= 0)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(productId));
            }

            await using var command = new SqlCommand("DeleteProduct", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            SetParameter(command, productId, "@productID", SqlDbType.Int, isNullable: false);

            if (this.connection.State != ConnectionState.Open)
            {
                await this.connection.OpenAsync();
            }

            var result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }

        /// <inheritdoc/>
        public async Task<ProductTransferObject> FindProductAsync(int productId)
        {
            if (productId <= 0)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(productId));
            }

            await using var command = new SqlCommand("FindProduct", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            SetParameter(command, productId, "@productID", SqlDbType.Int, isNullable: false);

            if (this.connection.State != ConnectionState.Open)
            {
                await this.connection.OpenAsync();
            }

            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                throw new ProductNotFoundException(productId);
            }

            return CreateProduct(reader);
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<ProductTransferObject> SelectProductsAsync(int offset, int limit)
        {
            if (offset < 0)
            {
                throw new ArgumentException("Must be greater than zero or equals zero.", nameof(offset));
            }

            if (limit < 1)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(limit));
            }

            await foreach (var product in SelectProductsAsync(offset, limit))
            {
                yield return product;
            }

            async IAsyncEnumerable<ProductTransferObject> SelectProductsAsync(int offset, int limit)
            {
                await using var command = new SqlCommand("SelectProducts", this.connection)
                {
                    CommandType = CommandType.StoredProcedure,
                };

                SetParameter(command, offset, "@offset", SqlDbType.Int, isNullable: false);
                SetParameter(command, limit, "@limit", SqlDbType.Int, isNullable: false);

                if (this.connection.State != ConnectionState.Open)
                {
                    await this.connection.OpenAsync();
                }

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    yield return CreateProduct(reader);
                }
            }
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ProductTransferObject> SelectProductsByNameAsync(IEnumerable<string> productNames)
        {
            if (productNames == null)
            {
                throw new ArgumentNullException(nameof(productNames));
            }

            if (productNames.Any())
            {
                throw new ArgumentException("Collection is empty.", nameof(productNames));
            }

            foreach (var name in productNames)
            {
                await foreach (var product in SelectProductsByNameAsync(name))
                {
                    yield return product;
                }
            }

            async IAsyncEnumerable<ProductTransferObject> SelectProductsByNameAsync(string productName)
            {
                await using var command = new SqlCommand("SelectProductsByName", this.connection)
                {
                    CommandType = CommandType.StoredProcedure,
                };

                SetParameter(command, productName, "@productName", SqlDbType.NVarChar, 40, false);

                if (this.connection.State != ConnectionState.Open)
                {
                    await this.connection.OpenAsync();
                }

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    yield return CreateProduct(reader);
                }
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateProductAsync(int productId, ProductTransferObject product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            await using var command = new SqlCommand("UpdateProduct", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            SetParameter(command, productId, "@productId", SqlDbType.Int, isNullable: false);
            AddSqlParameters(product, command);

            if (this.connection.State != ConnectionState.Open)
            {
                await this.connection.OpenAsync();
            }

            var result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ProductTransferObject> SelectProductByCategoryAsync(IEnumerable<int> collectionOfCategoryId)
        {
            if (collectionOfCategoryId == null)
            {
                throw new ArgumentNullException(nameof(collectionOfCategoryId));
            }

            foreach (var categoryId in collectionOfCategoryId)
            {
                await foreach (var product in SelectProductByCategoryAsync(categoryId))
                {
                    yield return product;
                }
            }

            async IAsyncEnumerable<ProductTransferObject> SelectProductByCategoryAsync(int categoryId)
            {
                await using var command = new SqlCommand("SelectProductsByCategory", this.connection)
                {
                    CommandType = CommandType.StoredProcedure,
                };

                SetParameter(command, categoryId, "@categoryId", SqlDbType.Int, isNullable: false);

                if (this.connection.State != ConnectionState.Open)
                {
                    await this.connection.OpenAsync();
                }

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    yield return CreateProduct(reader);
                }
            }
        }

        private static ProductTransferObject CreateProduct(SqlDataReader reader)
        {
            return new ProductTransferObject
            {
                Id = (int)reader["ProductID"],
                Name = (string)reader["ProductName"],
                SupplierId = GetValueStruct<int>("SupplierID"),
                CategoryId = GetValueStruct<int>("CategoryID"),
                QuantityPerUnit = GetValueClass<string>("QuantityPerUnit"),
                UnitPrice = GetValueStruct<decimal>("UnitPrice"),
                UnitsInStock = GetValueStruct<short>("UnitsInStock"),
                UnitsOnOrder = GetValueStruct<short>("UnitsOnOrder"),
                ReorderLevel = GetValueStruct<short>("ReorderLevel"),
                Discontinued = (bool)reader["Discontinued"],
            };

            T GetValueClass<T>(string text)
                where T : class
                => reader[text] == DBNull.Value ? null : (T)reader[text];

            T? GetValueStruct<T>(string text)
                where T : struct
                => reader[text] == DBNull.Value ? null : (T)reader[text];
        }

        private static void AddSqlParameters(ProductTransferObject product, SqlCommand command)
        {
            SetParameter(command, product.Name, "@productName", SqlDbType.NVarChar, 40, false);
            SetParameter(command, product.SupplierId, "@supplierId", SqlDbType.Int);
            SetParameter(command, product.CategoryId, "@categoryId", SqlDbType.Int);
            SetParameter(command, product.QuantityPerUnit, "@quantityPerUnit", SqlDbType.NVarChar, 20);
            SetParameter(command, product.UnitPrice, "@unitPrice", SqlDbType.Money);
            SetParameter(command, product.UnitsInStock, "@unitsInStock", SqlDbType.SmallInt);
            SetParameter(command, product.UnitsOnOrder, "@unitsOnOrder", SqlDbType.SmallInt);
            SetParameter(command, product.ReorderLevel, "@reorderLevel", SqlDbType.SmallInt);
            SetParameter(command, product.Discontinued, "@discontinued", SqlDbType.Bit, isNullable: false);
        }

        private static void SetParameter<T>(SqlCommand command, T property, string parameterName, SqlDbType dbType, int? size = null, bool isNullable = true)
        {
            if (size is null)
            {
                command.Parameters.Add(parameterName, dbType);
            }
            else
            {
                command.Parameters.Add(parameterName, dbType, (int)size);
            }

            command.Parameters[parameterName].IsNullable = isNullable;
            command.Parameters[parameterName].Value = property ?? Convert.DBNull;
        }
    }
}
