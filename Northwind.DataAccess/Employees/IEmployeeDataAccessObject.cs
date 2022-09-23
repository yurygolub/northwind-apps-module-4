using System.Collections.Generic;
using System.Threading.Tasks;

namespace Northwind.DataAccess.Employees
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
        Task<int> InsertEmployeeAsync(EmployeeTransferObject employee);

        /// <summary>
        /// Deletes a Northwind employee from a data storage.
        /// </summary>
        /// <param name="employeeId">An employee identifier.</param>
        /// <returns>True if an employee is deleted; otherwise false.</returns>
        Task<bool> DeleteEmployeeAsync(int employeeId);

        /// <summary>
        /// Updates a Northwind employee in a data storage.
        /// </summary>
        /// <param name="employeeId">A data storage identifier of an existed employee.</param>
        /// <param name="employee">An <see cref="EmployeeTransferObject"/>.</param>
        /// <returns>True if an employee is updated; otherwise false.</returns>
        Task<bool> UpdateEmployeeAsync(int employeeId, EmployeeTransferObject employee);

        /// <summary>
        /// Finds a Northwind employee using a specified identifier.
        /// </summary>
        /// <param name="employeeId">A data storage identifier of an existed employee.</param>
        /// <returns>An <see cref="EmployeeTransferObject"/> with specified identifier.</returns>
        Task<EmployeeTransferObject> FindEmployeeAsync(int employeeId);

        /// <summary>
        /// Selects employees using specified offset and limit.
        /// </summary>
        /// <param name="offset">An offset of the first object.</param>
        /// <param name="limit">A limit of returned objects.</param>
        /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="EmployeeTransferObject"/>.</returns>
        IAsyncEnumerable<EmployeeTransferObject> SelectEmployeesAsync(int offset, int limit);
    }
}
