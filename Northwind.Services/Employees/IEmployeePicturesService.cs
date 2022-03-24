using System.IO;
using System.Threading.Tasks;

namespace Northwind.Services.Employees
{
    /// <summary>
    /// Represents a management service for employee pictures.
    /// </summary>
    public interface IEmployeePicturesService
    {
        /// <summary>
        /// Gets a employee picture with specified identifier.
        /// </summary>
        /// <param name="employeeId">An employee identifier.</param>
        /// <returns>Returns an employee picture <see cref="Stream"/>.</returns>
        Task<Stream> GetEmployeePictureAsync(int employeeId);

        /// <summary>
        /// Deletes an existed employee picture.
        /// </summary>
        /// <param name="employeeId">An employee identifier.</param>
        /// <returns>True if an employee picture is destroyed; otherwise false.</returns>
        Task<bool> DeleteEmployeePictureAsync(int employeeId);

        /// <summary>
        /// Updates an employee picture.
        /// </summary>
        /// <param name="employeeId">An employee identifier.</param>
        /// <param name="stream">A <see cref="Stream"/>.</param>
        /// <returns>True if an employee picture is updated; otherwise false.</returns>
        Task<bool> UpdateEmployeePictureAsync(int employeeId, Stream stream);
    }
}
