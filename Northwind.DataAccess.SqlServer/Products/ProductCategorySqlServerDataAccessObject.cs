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
    /// Represents a SQL Server-tailored DAO for Northwind product categories.
    /// </summary>
    public sealed class ProductCategorySqlServerDataAccessObject : IProductCategoryDataAccessObject
    {
        private readonly SqlConnection connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCategorySqlServerDataAccessObject"/> class.
        /// </summary>
        /// <param name="connection">A <see cref="SqlConnection"/>.</param>
        public ProductCategorySqlServerDataAccessObject(SqlConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        /// <inheritdoc/>
        public async Task<int> InsertProductCategoryAsync(ProductCategoryTransferObject productCategory)
        {
            if (productCategory == null)
            {
                throw new ArgumentNullException(nameof(productCategory));
            }

            using var command = new SqlCommand("InsertProductCategory", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            AddSqlParameters(productCategory, command);

            if (this.connection.State != ConnectionState.Open)
            {
                await this.connection.OpenAsync();
            }

            return await command.ExecuteNonQueryAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteProductCategoryAsync(int productCategoryId)
        {
            if (productCategoryId <= 0)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(productCategoryId));
            }

            await using var command = new SqlCommand("DeleteProductCategory", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            SetParameter(command, productCategoryId, "@categoryID", SqlDbType.Int, isNullable: false);

            if (this.connection.State != ConnectionState.Open)
            {
                await this.connection.OpenAsync();
            }

            var result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }

        /// <inheritdoc/>
        public async Task<ProductCategoryTransferObject> FindProductCategoryAsync(int productCategoryId)
        {
            if (productCategoryId <= 0)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(productCategoryId));
            }

            using var command = new SqlCommand("FindProductCategory", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            SetParameter(command, productCategoryId, "@categoryID", SqlDbType.Int, isNullable: false);

            if (this.connection.State != ConnectionState.Open)
            {
                await this.connection.OpenAsync();
            }

            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                throw new ProductCategoryNotFoundException(productCategoryId);
            }

            return CreateProductCategory(reader);
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ProductCategoryTransferObject> SelectProductCategoriesAsync(int offset, int limit)
        {
            if (offset < 0)
            {
                throw new ArgumentException("Must be greater than zero or equals zero.", nameof(offset));
            }

            if (limit < 1)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(limit));
            }

            await foreach (var productCategory in SelectProductsAsync(offset, limit))
            {
                yield return productCategory;
            }

            async IAsyncEnumerable<ProductCategoryTransferObject> SelectProductsAsync(int offset, int limit)
            {
                await using var command = new SqlCommand("SelectProductCategories", this.connection)
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
                    yield return CreateProductCategory(reader);
                }
            }
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ProductCategoryTransferObject> SelectProductCategoriesByNameAsync(IEnumerable<string> productCategoryNames)
        {
            if (productCategoryNames == null)
            {
                throw new ArgumentNullException(nameof(productCategoryNames));
            }

            if (productCategoryNames.Any())
            {
                throw new ArgumentException("Collection is empty.", nameof(productCategoryNames));
            }

            foreach (var name in productCategoryNames)
            {
                await foreach (var productCategory in SelectProductsByNameAsync(name))
                {
                    yield return productCategory;
                }
            }

            async IAsyncEnumerable<ProductCategoryTransferObject> SelectProductsByNameAsync(string productCategoryName)
            {
                await using var command = new SqlCommand("SelectProductCategoriesByName", this.connection)
                {
                    CommandType = CommandType.StoredProcedure,
                };

                SetParameter(command, productCategoryName, "@categoryName", SqlDbType.NVarChar, 15, false);

                if (this.connection.State != ConnectionState.Open)
                {
                    await this.connection.OpenAsync();
                }

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    yield return CreateProductCategory(reader);
                }
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateProductCategoryAsync(int productCategoryId, ProductCategoryTransferObject productCategory)
        {
            if (productCategory == null)
            {
                throw new ArgumentNullException(nameof(productCategory));
            }

            using var command = new SqlCommand("UpdateProductCategory", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            SetParameter(command, productCategoryId, "@categoryID", SqlDbType.Int, isNullable: false);
            AddSqlParameters(productCategory, command);

            if (this.connection.State != ConnectionState.Open)
            {
                await this.connection.OpenAsync();
            }

            var result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }

        private static ProductCategoryTransferObject CreateProductCategory(SqlDataReader reader)
        {
            return new ProductCategoryTransferObject
            {
                Id = (int)reader["CategoryID"],
                Name = (string)reader["CategoryName"],
                Description = GetValueClass<string>("Description"),
                Picture = GetValueClass<byte[]>("Picture"),
            };

            T GetValueClass<T>(string text)
                where T : class
                => reader[text] == DBNull.Value ? null : (T)reader[text];
        }

        private static void AddSqlParameters(ProductCategoryTransferObject productCategory, SqlCommand command)
        {
            SetParameter(command, productCategory.Name, "@categoryName", SqlDbType.NVarChar, 15, false);
            SetParameter(command, productCategory.Description, "@description", SqlDbType.NText);
            SetParameter(command, productCategory.Picture, "@picture", SqlDbType.Image);
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
