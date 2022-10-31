using System;
using System.IO;
using System.Threading.Tasks;
using Northwind.DataAccess;
using Northwind.DataAccess.Employees;
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
            _ = northwindDataAccessFactory ?? throw new ArgumentNullException(nameof(northwindDataAccessFactory));

            this.dataAccessObject = northwindDataAccessFactory.GetEmployeeDataAccessObject();
        }

        public async Task<Stream> GetEmployeePictureAsync(int employeeId)
        {
            try
            {
                var employee = await this.dataAccessObject.FindEmployeeAsync(employeeId);
                if (employee?.Photo is null)
                {
                    return null;
                }

                return new MemoryStream(employee.Photo[78..]);
            }
            catch (EmployeeNotFoundException)
            {
                return null;
            }
        }

        public async Task<bool> DeleteEmployeePictureAsync(int employeeId)
        {
            var employee = await this.dataAccessObject.FindEmployeeAsync(employeeId);
            if (employee is null)
            {
                return false;
            }

            employee.Photo = null;

            return await this.dataAccessObject.UpdateEmployeeAsync(employeeId, employee);
        }

        public async Task<bool> UpdateEmployeePictureAsync(int employeeId, Stream stream)
        {
            _ = stream ?? throw new ArgumentNullException(nameof(stream));

            var employee = await this.dataAccessObject.FindEmployeeAsync(employeeId);
            if (employee is null)
            {
                return false;
            }

            await using MemoryStream memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.ToArray().CopyTo(employee.Photo, 78);

            return await this.dataAccessObject.UpdateEmployeeAsync(employeeId, employee);
        }
    }
}
