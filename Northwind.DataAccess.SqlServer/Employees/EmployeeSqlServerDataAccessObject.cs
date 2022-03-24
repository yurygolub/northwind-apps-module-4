using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
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
        public async Task<int> InsertEmployeeAsync(EmployeeTransferObject employee)
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

            if (this.connection.State != ConnectionState.Open)
            {
                await this.connection.OpenAsync();
            }

            return await command.ExecuteNonQueryAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteEmployeeAsync(int employeeId)
        {
            if (employeeId <= 0)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(employeeId));
            }

            using var command = new SqlCommand("DeleteEmployee", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            SetParameter(command, employeeId, "@categoryID", SqlDbType.Int, isNullable: false);

            if (this.connection.State != ConnectionState.Open)
            {
                await this.connection.OpenAsync();
            }

            var result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }

        /// <inheritdoc/>
        public async Task<EmployeeTransferObject> FindEmployeeAsync(int employeeId)
        {
            if (employeeId <= 0)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(employeeId));
            }

            using var command = new SqlCommand("FindEmployee", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            SetParameter(command, employeeId, "@employeeID", SqlDbType.Int, isNullable: false);

            if (this.connection.State != ConnectionState.Open)
            {
                await this.connection.OpenAsync();
            }

            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
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

            await foreach (var product in SelectEmployeesAsync(offset, limit))
            {
                yield return product;
            }

            async IAsyncEnumerable<EmployeeTransferObject> SelectEmployeesAsync(int offset, int limit)
            {
                await using var command = new SqlCommand("SelectEmployees", this.connection)
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
                    yield return CreateEmployee(reader);
                }
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateEmployeeAsync(int employeeId, EmployeeTransferObject employee)
        {
            if (employee == null)
            {
                throw new ArgumentNullException(nameof(employee));
            }

            using var command = new SqlCommand("UpdateEmployee", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            SetParameter(command, employeeId, "@employeeID", SqlDbType.Int, isNullable: false);
            AddSqlParameters(employee, command);

            if (this.connection.State != ConnectionState.Open)
            {
                await this.connection.OpenAsync();
            }

            var result = await command.ExecuteNonQueryAsync();
            return result > 0;
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
            SetParameter(command, employee.LastName, "@lastName", SqlDbType.NVarChar, 20, false);
            SetParameter(command, employee.FirstName, "@firstName", SqlDbType.NVarChar, 10, false);
            SetParameter(command, employee.Title, "@title", SqlDbType.NVarChar, 30);
            SetParameter(command, employee.TitleOfCourtesy, "@titleOfCourtesy", SqlDbType.NVarChar, 25);
            SetParameter(command, employee.BirthDate, "@birthDate", SqlDbType.DateTime);
            SetParameter(command, employee.HireDate, "@hireDate", SqlDbType.DateTime);
            SetParameter(command, employee.Address, "@address", SqlDbType.NVarChar, 60);
            SetParameter(command, employee.City, "@city", SqlDbType.NVarChar, 15);
            SetParameter(command, employee.Region, "@region", SqlDbType.NVarChar, 15);
            SetParameter(command, employee.PostalCode, "@postalCode", SqlDbType.NVarChar, 10);
            SetParameter(command, employee.Country, "@country", SqlDbType.NVarChar, 15);
            SetParameter(command, employee.HomePhone, "@homePhone", SqlDbType.NVarChar, 24);
            SetParameter(command, employee.Extension, "@extension", SqlDbType.NVarChar, 4);
            SetParameter(command, employee.Photo, "@photo", SqlDbType.Image);
            SetParameter(command, employee.Notes, "@notes", SqlDbType.NText);
            SetParameter(command, employee.ReportsTo, "@reportsTo", SqlDbType.Int);
            SetParameter(command, employee.PhotoPath, "@photoPath", SqlDbType.NVarChar, 255);
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
