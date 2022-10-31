using System;
using System.IO;
using System.Threading.Tasks;
using Northwind.Services.Employees;

namespace Northwind.Services.EntityFrameworkCore.Employees
{
    public class EmployeePicturesService : IEmployeePicturesService
    {
        private readonly string connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeePicturesService"/> class.
        /// </summary>
        public EmployeePicturesService(string connectionString)
        {
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<Stream> GetEmployeePictureAsync(int employeeId)
        {
            await using Models.NorthwindContext db = new Models.NorthwindContext(this.connectionString);
            var employee = await db.Employees.FindAsync(employeeId);
            if (employee?.Photo is null)
            {
                return null;
            }

            return new MemoryStream(employee.Photo[78..]);
        }

        public async Task<bool> DeleteEmployeePictureAsync(int employeeId)
        {
            await using Models.NorthwindContext db = new Models.NorthwindContext(this.connectionString);
            var employee = await db.Employees.FindAsync(employeeId);
            if (employee is null)
            {
                return false;
            }

            employee.Photo = null;

            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateEmployeePictureAsync(int employeeId, Stream stream)
        {
            _ = stream ?? throw new ArgumentNullException(nameof(stream));

            await using Models.NorthwindContext db = new Models.NorthwindContext(this.connectionString);
            var employee = await db.Employees.FindAsync(employeeId);
            if (employee is null)
            {
                return false;
            }

            await using MemoryStream memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.ToArray().CopyTo(employee.Photo, 78);

            await db.SaveChangesAsync();

            return true;
        }
    }
}
