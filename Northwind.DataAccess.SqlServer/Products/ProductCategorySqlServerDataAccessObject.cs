using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
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
        public int InsertProductCategory(ProductCategoryTransferObject productCategory)
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

            var id = command.ExecuteScalar();
            return (int)id;
        }

        /// <inheritdoc/>
        public bool DeleteProductCategory(int productCategoryId)
        {
            if (productCategoryId <= 0)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(productCategoryId));
            }

            using var command = new SqlCommand("DeleteProductCategory", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };
            const string productIdParameter = "@categoryID";
            command.Parameters.Add(productIdParameter, SqlDbType.Int);
            command.Parameters[productIdParameter].Value = productCategoryId;

            var result = command.ExecuteScalar();
            return ((int)result) > 0;
        }

        /// <inheritdoc/>
        public ProductCategoryTransferObject FindProductCategory(int productCategoryId)
        {
            if (productCategoryId <= 0)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(productCategoryId));
            }

            using var command = new SqlCommand("FindProductCategory", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };
            const string productIdParameter = "@categoryID";
            command.Parameters.Add(productIdParameter, SqlDbType.Int);
            command.Parameters[productIdParameter].Value = productCategoryId;

            using var reader = command.ExecuteReader();

            if (!reader.Read())
            {
                throw new ProductNotFoundException(productCategoryId);
            }

            return CreateProductCategory(reader);
        }

        /// <inheritdoc/>
        public async Task<IList<ProductCategoryTransferObject>> SelectProductCategoriesAsync(int offset, int limit)
        {
            if (offset < 0)
            {
                throw new ArgumentException("Must be greater than zero or equals zero.", nameof(offset));
            }

            if (limit < 1)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(limit));
            }

            using var command = new SqlCommand("SelectProductCategories", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            const string offsetParameter = "@offset";
            command.Parameters.Add(offsetParameter, SqlDbType.Int);
            command.Parameters[offsetParameter].Value = offset;

            const string limitParameter = "@limit";
            command.Parameters.Add(limitParameter, SqlDbType.Int);
            command.Parameters[limitParameter].Value = limit;

            using var reader = await command.ExecuteReaderAsync();

            var productCategories = new List<ProductCategoryTransferObject>();
            while (reader.Read())
            {
                productCategories.Add(CreateProductCategory(reader));
            }

            return productCategories;
        }

        /// <inheritdoc/>
        public async Task<IList<ProductCategoryTransferObject>> SelectProductCategoriesByNameAsync(ICollection<string> productCategoryNames)
        {
            if (productCategoryNames == null)
            {
                throw new ArgumentNullException(nameof(productCategoryNames));
            }

            if (productCategoryNames.Count < 1)
            {
                throw new ArgumentException("Collection is empty.", nameof(productCategoryNames));
            }

            const string commandTemplate =
@"SELECT c.CategoryID, c.CategoryName, c.Description, c.Picture FROM dbo.Categories as c
WHERE c.CategoryName in ('{0}')
ORDER BY c.CategoryID";

            string commandText = string.Format(CultureInfo.CurrentCulture, commandTemplate, string.Join("', '", productCategoryNames));
            return await this.ExecuteReaderAsync(commandText);
        }

        /// <inheritdoc/>
        public bool UpdateProductCategory(ProductCategoryTransferObject productCategory)
        {
            if (productCategory == null)
            {
                throw new ArgumentNullException(nameof(productCategory));
            }

            using var command = new SqlCommand("UpdateProductCategory", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            AddSqlParameters(productCategory, command);

            const string productId = "@categoryID";
            command.Parameters.Add(productId, SqlDbType.Int);
            command.Parameters[productId].Value = productCategory.Id;

            var result = command.ExecuteScalar();
            return ((int)result) > 0;
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
            SetParameter(productCategory.Name, "@categoryName", SqlDbType.NVarChar, 15, false);
            SetParameter(productCategory.Description, "@description", SqlDbType.NText);
            SetParameter(productCategory.Picture, "@picture", SqlDbType.Image);

            void SetParameter<T>(T property, string parameterName, SqlDbType dbType, int? size = null, bool isNullable = true)
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
                command.Parameters[parameterName].Value = property != null ? property : DBNull.Value;
            }
        }

        private async Task<IList<ProductCategoryTransferObject>> ExecuteReaderAsync(string commandText)
        {
            var productCategories = new List<ProductCategoryTransferObject>();
            using var command = new SqlCommand(commandText, this.connection);
            using var reader = await command.ExecuteReaderAsync();
            
            while (reader.Read())
            {
                productCategories.Add(CreateProductCategory(reader));
            }

            return productCategories;
        }
    }
}
