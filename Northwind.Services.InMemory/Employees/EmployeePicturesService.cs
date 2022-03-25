using System;
using System.IO;
using System.Threading.Tasks;
using Northwind.Services.Employees;

namespace Northwind.Services.InMemory.Employees
{
    public class EmployeePicturesService : IEmployeePicturesService
    {
        private readonly NorthwindContext northwindContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeePicturesService"/> class.
        /// </summary>
        /// <param name="northwindContext">NorthwindContext.</param>
        /// <exception cref="ArgumentNullException">Thrown if northwindContext is null.</exception>
        public EmployeePicturesService(NorthwindContext northwindContext)
        {
            if (northwindContext is null)
            {
                throw new ArgumentNullException(nameof(northwindContext));
            }

            this.northwindContext = northwindContext;
        }

        public async Task<Stream> GetEmployeePictureAsync(int employeeId)
        {
            var employee = await this.northwindContext.Employees.FindAsync(employeeId);
            if (employee?.Photo is null)
            {
                return null;
            }

            return new MemoryStream(employee.Photo[78..]);
        }

        public async Task<bool> DeleteEmployeePictureAsync(int employeeId)
        {
            var employee = await this.northwindContext.Employees.FindAsync(employeeId);
            if (employee is null)
            {
                return false;
            }

            employee.Photo = null;

            await this.northwindContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateEmployeePictureAsync(int employeeId, Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var employee = await this.northwindContext.Employees.FindAsync(employeeId);
            if (employee is null)
            {
                return false;
            }

            await using MemoryStream memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.ToArray().CopyTo(employee.Photo, 78);

            await this.northwindContext.SaveChangesAsync();

            return true;
        }
    }
}
