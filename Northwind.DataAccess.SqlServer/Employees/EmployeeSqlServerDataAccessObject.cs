using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Northwind.Services.Employees;

#pragma warning disable S4457

namespace Northwind.Services.SqlServer.Employees
{
    /// <summary>
    /// Represents a SQL Server-tailored DAO for Northwind products.
    /// </summary>
    public sealed class EmployeeSqlServerDataAccessObject : IEmployeeDataAccessObject
    {
        private readonly SqlConnection connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeSqlServerDataAccessObject"/> class.
        /// </summary>
        /// <param name="connection">A <see cref="SqlConnection"/>.</param>
        public EmployeeSqlServerDataAccessObject(SqlConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));

        }

        /// <inheritdoc/>
        public int InsertEmployee(EmployeeTransferObject employee)
        {
            if (employee == null)
            {
                throw new ArgumentNullException(nameof(employee));
            }

            using var command = new SqlCommand("InsertEmployee", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            AddSqlParameters(employee, command);

            var id = command.ExecuteScalar();
            return (int)id;
        }

        /// <inheritdoc/>
        public bool DeleteEmployee(int employeeId)
        {
            if (employeeId <= 0)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(employeeId));
            }

            using var command = new SqlCommand("DeleteEmployee", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };
            const string productIdParameter = "@categoryID";
            command.Parameters.Add(productIdParameter, SqlDbType.Int);
            command.Parameters[productIdParameter].Value = employeeId;

            var result = command.ExecuteScalar();
            return ((int)result) > 0;
        }

        /// <inheritdoc/>
        public EmployeeTransferObject FindEmployee(int employeeId)
        {
            if (employeeId <= 0)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(employeeId));
            }

            using var command = new SqlCommand("FindEmployee", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };
            const string productIdParameter = "@employeeID";
            command.Parameters.Add(productIdParameter, SqlDbType.Int);
            command.Parameters[productIdParameter].Value = employeeId;

            using var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                throw new EmployeeNotFoundException(employeeId);
            }

            return CreateEmployee(reader);
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<EmployeeTransferObject> SelectEmployeesAsync(int offset, int limit)
        {
            if (offset < 0)
            {
                throw new ArgumentException("Must be greater than zero or equals zero.", nameof(offset));
            }

            if (limit < 1)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(limit));
            }

            using var command = new SqlCommand("SelectEmployees", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            const string offsetParameter = "@offset";
            command.Parameters.Add(offsetParameter, SqlDbType.Int);
            command.Parameters[offsetParameter].Value = offset;

            const string limitParameter = "@limit";
            command.Parameters.Add(limitParameter, SqlDbType.Int);
            command.Parameters[limitParameter].Value = limit;

            await using var reader = await command.ExecuteReaderAsync();

            while (reader.Read())
            {
                yield return CreateEmployee(reader);
            }
        }

        /// <inheritdoc/>
        public bool UpdateEmployee(EmployeeTransferObject employee)
        {
            if (employee == null)
            {
                throw new ArgumentNullException(nameof(employee));
            }

            using var command = new SqlCommand("UpdateEmployee", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            AddSqlParameters(employee, command);

            const string productId = "@employeeID";
            command.Parameters.Add(productId, SqlDbType.Int);
            command.Parameters[productId].Value = employee.EmployeeID;

            var result = command.ExecuteScalar();
            return ((int)result) > 0;
        }

        private static EmployeeTransferObject CreateEmployee(SqlDataReader reader)
        {
            return new EmployeeTransferObject
            {
                EmployeeID = (int)reader["EmployeeID"],
                LastName = (string)reader["LastName"],
                FirstName = (string)reader["FirstName"],
                Title = GetValueClass<string>("Title"),
                TitleOfCourtesy = GetValueClass<string>("TitleOfCourtesy"),
                BirthDate = GetValueStruct<DateTime>("BirthDate"),
                HireDate = GetValueStruct<DateTime>("HireDate"),
                Address = GetValueClass<string>("Address"),
                City = GetValueClass<string>("City"),
                Region = GetValueClass<string>("Region"),
                PostalCode = GetValueClass<string>("PostalCode"),
                Country = GetValueClass<string>("Country"),
                HomePhone = GetValueClass<string>("HomePhone"),
                Extension = GetValueClass<string>("Extension"),
                Photo = GetValueClass<byte[]>("Photo"),
                Notes = GetValueClass<string>("Notes"),
                ReportsTo = GetValueStruct<int>("ReportsTo"),
                PhotoPath = GetValueClass<string>("PhotoPath"),
            };

            T GetValueClass<T>(string text)
                where T : class
                => reader[text] == DBNull.Value ? null : (T)reader[text];

            T? GetValueStruct<T>(string text)
                where T : struct
                => reader[text] == DBNull.Value ? null : (T)reader[text];
        }

        private static void AddSqlParameters(EmployeeTransferObject employee, SqlCommand command)
        {
            SetParameter(employee.LastName, "@lastName", SqlDbType.NVarChar, 20, false);
            SetParameter(employee.FirstName, "@firstName", SqlDbType.NVarChar, 10, false);
            SetParameter(employee.Title, "@title", SqlDbType.NVarChar, 30);
            SetParameter(employee.TitleOfCourtesy, "@titleOfCourtesy", SqlDbType.NVarChar, 25);
            SetParameter(employee.BirthDate, "@birthDate", SqlDbType.DateTime);
            SetParameter(employee.HireDate, "@hireDate", SqlDbType.DateTime);
            SetParameter(employee.Address, "@address", SqlDbType.NVarChar, 60);
            SetParameter(employee.City, "@city", SqlDbType.NVarChar, 15);
            SetParameter(employee.Region, "@region", SqlDbType.NVarChar, 15);
            SetParameter(employee.PostalCode, "@postalCode", SqlDbType.NVarChar, 10);
            SetParameter(employee.Country, "@country", SqlDbType.NVarChar, 15);
            SetParameter(employee.HomePhone, "@homePhone", SqlDbType.NVarChar, 24);
            SetParameter(employee.Extension, "@extension", SqlDbType.NVarChar, 4);
            SetParameter(employee.Photo, "@photo", SqlDbType.Image);
            SetParameter(employee.Notes, "@notes", SqlDbType.NText);
            SetParameter(employee.ReportsTo, "@reportsTo", SqlDbType.Int);
            SetParameter(employee.PhotoPath, "@photoPath", SqlDbType.NVarChar, 255);

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
    }
}
