﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Northwind.Services.Employees
{
    /// <summary>
    /// Represents a management service for employees.
    /// </summary>
    public interface IEmployeeManagementService
    {
        /// <summary>
        /// Shows a list of employees using specified offset and limit for pagination.
        /// </summary>
        /// <param name="offset">An offset of the first element to return.</param>
        /// <param name="limit">A limit of elements to return.</param>
        /// <returns>A <see cref="IList{T}"/> of <see cref="Product"/>.</returns>
        Task<IList<Employee>> ShowEmployeesAsync(int offset, int limit);

        /// <summary>
        /// Try to show a employee with specified identifier.
        /// </summary>
        /// <param name="employeeId">An employee identifier.</param>
        /// <param name="employee">An employee to return.</param>
        /// <returns>Returns true if a product is returned; otherwise false.</returns>
        bool TryShowEmployee(int employeeId, out Employee employee);

        /// <summary>
        /// Creates a new employee.
        /// </summary>
        /// <param name="employee">A <see cref="Employee"/> to create.</param>
        /// <returns>An identifier of a created employee.</returns>
        int CreateEmployee(Employee employee);

        /// <summary>
        /// Destroys an existed employee.
        /// </summary>
        /// <param name="employeeId">An employee identifier.</param>
        /// <returns>True if an employee is destroyed; otherwise false.</returns>
        bool DestroyEmployee(int employeeId);

        /// <summary>
        /// Updates an employee.
        /// </summary>
        /// <param name="employeeId">An employee identifier.</param>
        /// <param name="employee">An <see cref="Employee"/>.</param>
        /// <returns>True if an employee is updated; otherwise false.</returns>
        bool UpdateEmployee(int employeeId, Employee employee);
    }
}
