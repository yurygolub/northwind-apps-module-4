using System;
using System.IO;
using System.Threading.Tasks;
using Northwind.Services.Employees;

namespace Northwind.Services.EntityFrameworkCore.Employees
{
    public class EmployeePicturesService : IEmployeePicturesService
    {
        private readonly Models.NorthwindContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeePicturesService"/> class.
        /// </summary>
        public EmployeePicturesService(Models.NorthwindContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Stream> GetEmployeePictureAsync(int employeeId)
        {
            var employee = await this.context.Employees.FindAsync(employeeId);
            if (employee?.Photo is null)
            {
                return null;
            }

            return new MemoryStream(employee.Photo[78..]);
        }

        public async Task<bool> DeleteEmployeePictureAsync(int employeeId)
        {
            var employee = await this.context.Employees.FindAsync(employeeId);
            if (employee is null)
            {
                return false;
            }

            employee.Photo = null;

            await this.context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateEmployeePictureAsync(int employeeId, Stream stream)
        {
            _ = stream ?? throw new ArgumentNullException(nameof(stream));

            var employee = await this.context.Employees.FindAsync(employeeId);
            if (employee is null)
            {
                return false;
            }

            await using MemoryStream memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.ToArray().CopyTo(employee.Photo, 78);

            await this.context.SaveChangesAsync();

            return true;
        }
    }
}
