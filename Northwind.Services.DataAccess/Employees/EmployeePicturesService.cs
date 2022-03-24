using System;
using System.IO;
using System.Threading.Tasks;
using Northwind.Services.Employees;

namespace Northwind.Services.DataAccess.Employees
{
    public class EmployeePicturesService : IEmployeePicturesService
    {
        private readonly IEmployeeDataAccessObject dataAccessObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeePicturesService"/> class.
        /// </summary>
        /// <param name="northwindDataAccessFactory">Factory for creating Northwind DAO.</param>
        public EmployeePicturesService(NorthwindDataAccessFactory northwindDataAccessFactory)
        {
            if (northwindDataAccessFactory is null)
            {
                throw new ArgumentNullException(nameof(northwindDataAccessFactory));
            }

            this.dataAccessObject = northwindDataAccessFactory.GetEmployeeDataAccessObject();
        }

        public async Task<Stream> GetEmployeePictureAsync(int employeeId)
        {
            var employee = await this.dataAccessObject.FindEmployeeAsync(employeeId);
            if (employee?.Photo is null)
            {
                return null;
            }

            return new MemoryStream(employee.Photo[78..]);
        }

        public async Task<bool> DeleteEmployeePictureAsync(int employeeId)
        {
            var employee = await this.dataAccessObject.FindEmployeeAsync(employeeId);
            if (employee is null)
            {
                return false;
            }

            employee.Photo = null;

            if (!await this.dataAccessObject.UpdateEmployeeAsync(employeeId, employee))
            {
                return false;
            }

            return true;
        }

        public async Task<bool> UpdateEmployeePictureAsync(int employeeId, Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var employee = await this.dataAccessObject.FindEmployeeAsync(employeeId);
            if (employee is null)
            {
                return false;
            }

            await using MemoryStream memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.ToArray().CopyTo(employee.Photo, 78);

            if (!await this.dataAccessObject.UpdateEmployeeAsync(employeeId, employee))
            {
                return false;
            }

            return true;
        }
    }
}
