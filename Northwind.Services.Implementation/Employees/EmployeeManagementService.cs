using System;
using System.Collections.Generic;
using Northwind.DataAccess;
using Northwind.DataAccess.Employees;
using Northwind.Services.Employees;

namespace Northwind.Services.Implementation.Employees
{
    /// <summary>
    /// Represents a management service for employees.
    /// </summary>
    public class EmployeeManagementService : IEmployeeManagementService
    {
        private readonly IEmployeeDataAccessObject dataAccessObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeManagementService"/> class.
        /// </summary>
        /// <param name="northwindDataAccessFactory">Factory for creating Northwind DAO.</param>
        public EmployeeManagementService(NorthwindDataAccessFactory northwindDataAccessFactory)
        {
            if (northwindDataAccessFactory is null)
            {
                throw new ArgumentNullException(nameof(northwindDataAccessFactory));
            }

            this.dataAccessObject = northwindDataAccessFactory.GetEmployeeDataAccessObject();
        }

        /// <inheritdoc/>
        public int CreateEmployee(Employee employee)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public bool DestroyEmployee(int employeeId)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public IList<Employee> ShowEmployees(int offset, int limit)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryShowEmployee(int employeeId, out Employee employee)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public bool UpdateEmployee(int employeeId, Employee employee)
        {
            throw new System.NotImplementedException();
        }
    }
}
