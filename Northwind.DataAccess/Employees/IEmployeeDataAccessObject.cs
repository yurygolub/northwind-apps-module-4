using System.Collections.Generic;
using System.Threading.Tasks;

namespace Northwind.Services.Employees
{
    /// <summary>
    /// Represents a DAO for Northwind employees.
    /// </summary>
    public interface IEmployeeDataAccessObject
    {
        /// <summary>
        /// Inserts a new Northwind employee to a data storage.
        /// </summary>
        /// <param name="employee">An <see cref="EmployeeTransferObject"/>.</param>
        /// <returns>A data storage identifier of a new employee.</returns>
        int InsertEmployee(EmployeeTransferObject employee);

        /// <summary>
        /// Deletes a Northwind employee from a data storage.
        /// </summary>
        /// <param name="employeeId">An employee identifier.</param>
        /// <returns>True if an employee is deleted; otherwise false.</returns>
        bool DeleteEmployee(int employeeId);

        /// <summary>
        /// Updates a Northwind employee in a data storage.
        /// </summary>
        /// <param name="employee">An <see cref="EmployeeTransferObject"/>.</param>
        /// <returns>True if an employee is updated; otherwise false.</returns>
        bool UpdateEmployee(EmployeeTransferObject employee);

        /// <summary>
        /// Finds a Northwind employee using a specified identifier.
        /// </summary>
        /// <param name="employeeId">A data storage identifier of an existed employee.</param>
        /// <returns>An <see cref="EmployeeTransferObject"/> with specified identifier.</returns>
        EmployeeTransferObject FindEmployee(int employeeId);

        /// <summary>
        /// Selects employees using specified offset and limit.
        /// </summary>
        /// <param name="offset">An offset of the first object.</param>
        /// <param name="limit">A limit of returned objects.</param>
        /// <returns>An <see cref="List{T}"/> of <see cref="EmployeeTransferObject"/>.</returns>
        Task<IList<EmployeeTransferObject>> SelectEmployeesAsync(int offset, int limit);
    }
}
