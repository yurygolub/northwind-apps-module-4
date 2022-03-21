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
        public int InsertProduct(ProductTransferObject product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            using var command = new SqlCommand("InsertProduct", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            AddSqlParameters(product, command);

            var id = command.ExecuteScalar();
            return (int)id;
        }

        /// <inheritdoc/>
        public bool DeleteProduct(int productId)
        {
            if (productId <= 0)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(productId));
            }

            using var command = new SqlCommand("DeleteProduct", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };
            const string productIdParameter = "@productID";
            command.Parameters.Add(productIdParameter, SqlDbType.Int);
            command.Parameters[productIdParameter].Value = productId;

            var result = command.ExecuteScalar();
            return ((int)result) > 0;
        }

        /// <inheritdoc/>
        public ProductTransferObject FindProduct(int productId)
        {
            if (productId <= 0)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(productId));
            }

            using var command = new SqlCommand("FindProduct", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };
            const string productIdParameter = "@productId";
            command.Parameters.Add(productIdParameter, SqlDbType.Int);
            command.Parameters[productIdParameter].Value = productId;

            using var reader = command.ExecuteReader();

            if (!reader.Read())
            {
                throw new ProductNotFoundException(productId);
            }

            return CreateProduct(reader);
        }

        /// <inheritdoc />
        public async Task<IList<ProductTransferObject>> SelectProducts(int offset, int limit)
        {
            if (offset < 0)
            {
                throw new ArgumentException("Must be greater than zero or equals zero.", nameof(offset));
            }

            if (limit < 1)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(limit));
            }

            using var command = new SqlCommand("SelectProducts", this.connection)
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

            var products = new List<ProductTransferObject>();
            while (reader.Read())
            {
                products.Add(CreateProduct(reader));
            }

            return products;
        }

        /// <inheritdoc/>
        public async Task<IList<ProductTransferObject>> SelectProductsByName(ICollection<string> productNames)
        {
            if (productNames == null)
            {
                throw new ArgumentNullException(nameof(productNames));
            }

            if (productNames.Count < 1)
            {
                throw new ArgumentException("Collection is empty.", nameof(productNames));
            }


            const string commandTemplate =
@"SELECT p.ProductID, p.ProductName, p.SupplierID, p.CategoryID, p.QuantityPerUnit, p.UnitPrice, p.UnitsInStock, p.UnitsOnOrder, p.ReorderLevel, p.Discontinued FROM dbo.Products as p
WHERE p.ProductName in ('{0}')
ORDER BY p.ProductID";

            string commandText = string.Format(CultureInfo.CurrentCulture, commandTemplate, string.Join("', '", productNames));
            return await this.ExecuteReader(commandText);
        }

        /// <inheritdoc/>
        public bool UpdateProduct(ProductTransferObject product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            using var command = new SqlCommand("UpdateProduct", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            AddSqlParameters(product, command);

            const string productId = "@productId";
            command.Parameters.Add(productId, SqlDbType.Int);
            command.Parameters[productId].Value = product.Id;

            var result = command.ExecuteScalar();
            return ((int)result) > 0;
        }

        /// <inheritdoc/>
        public async Task<IList<ProductTransferObject>> SelectProductByCategory(ICollection<int> collectionOfCategoryId)
        {
            if (collectionOfCategoryId == null)
            {
                throw new ArgumentNullException(nameof(collectionOfCategoryId));
            }

            var whereInClause = string.Join("','", collectionOfCategoryId.Select(id => string.Format(CultureInfo.InvariantCulture, "{0:d}", id)).ToArray());

            const string commandTemplate =
@"SELECT p.ProductID, p.ProductName, p.SupplierID, p.CategoryID, p.QuantityPerUnit, p.UnitPrice, p.UnitsInStock, p.UnitsOnOrder, p.ReorderLevel, p.Discontinued
FROM dbo.Products as p
WHERE p.CategoryID in ('{0}')";

            string commandText = string.Format(CultureInfo.InvariantCulture, commandTemplate, whereInClause);

            var products = new List<ProductTransferObject>();
            await using var command = new SqlCommand(commandText, this.connection);
            await using var reader = await command.ExecuteReaderAsync();
            
            while (reader.Read())
            {
                products.Add(CreateProduct(reader));
            }
            
            return products;
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
            SetParameter(product.Name, "@productName", SqlDbType.NVarChar, 40, false);
            SetParameter(product.SupplierId, "@supplierId", SqlDbType.Int);
            SetParameter(product.CategoryId, "@categoryId", SqlDbType.Int);
            SetParameter(product.QuantityPerUnit, "@quantityPerUnit", SqlDbType.NVarChar, 20);
            SetParameter(product.UnitPrice, "@unitPrice", SqlDbType.Money);
            SetParameter(product.UnitsInStock, "@unitsInStock", SqlDbType.SmallInt);
            SetParameter(product.UnitsOnOrder, "@unitsOnOrder", SqlDbType.SmallInt);
            SetParameter(product.ReorderLevel, "@reorderLevel", SqlDbType.SmallInt);
            SetParameter(product.Discontinued, "@discontinued", SqlDbType.Bit, isNullable: false);

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

        private async Task<IList<ProductTransferObject>> ExecuteReader(string commandText)
        {
            var products = new List<ProductTransferObject>();
            using var command = new SqlCommand(commandText, this.connection);
            using var reader = await command.ExecuteReaderAsync();

            while (reader.Read())
            {
                products.Add(CreateProduct(reader));
            }
            
            return products;
        }
    }
}
