using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Northwind.Services.Employees;

namespace Northwind.Services.InMemory.Employees
{
    /// <summary>
    /// Represents a management service for employees.
    /// </summary>
    public class EmployeeManagementService : IEmployeeManagementService
    {
        private readonly NorthwindContext northwindContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeManagementService"/> class.
        /// </summary>
        /// <param name="northwindContext">NorthwindContext.</param>
        /// <exception cref="ArgumentNullException">Thrown if northwindContext is null.</exception>
        public EmployeeManagementService(NorthwindContext northwindContext)
        {
            if (northwindContext is null)
            {
                throw new ArgumentNullException(nameof(northwindContext));
            }

            this.northwindContext = northwindContext;
        }

        public async Task<int> CreateEmployeeAsync(Employee employee)
        {
            if (employee is null)
            {
                throw new ArgumentNullException(nameof(employee));
            }

            await this.northwindContext.Employees.AddAsync(MapEmployee(employee));
            await this.northwindContext.SaveChangesAsync();
            return employee.EmployeeID;
        }

        public async Task<bool> DestroyEmployeeAsync(int employeeId)
        {
            var employee = await this.northwindContext.Employees.FindAsync(employeeId);
            if (employee != null)
            {
                this.northwindContext.Employees.Remove(employee);
                await this.northwindContext.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async IAsyncEnumerable<Employee> GetEmployeesAsync(int offset, int limit)
        {
            var employees = this.northwindContext.Employees
                    .Skip(offset)
                    .Take(limit)
                    .Select(e => MapEmployee(e));

            await foreach (var employee in employees.AsAsyncEnumerable())
            {
                yield return employee;
            }
        }

        public async Task<Employee> GetEmployeeAsync(int employeeId)
        {
            var contextEmployee = await this.northwindContext.Employees.FindAsync(employeeId);
            if (contextEmployee is null)
            {
                return null;
            }

            return MapEmployee(contextEmployee);
        }

        public async Task<bool> UpdateEmployeeAsync(int employeeId, Employee employee)
        {
            if (employee is null)
            {
                throw new ArgumentNullException(nameof(employee));
            }

            var contextEmployee = await this.northwindContext.Employees.FindAsync(employeeId);
            if (contextEmployee is null)
            {
                return false;
            }

            contextEmployee.Address = employee.Address;
            contextEmployee.BirthDate = employee.BirthDate;
            contextEmployee.City = employee.City;
            contextEmployee.Country = employee.Country;
            contextEmployee.Extension = employee.Extension;
            contextEmployee.FirstName = employee.FirstName;
            contextEmployee.LastName = employee.LastName;
            contextEmployee.HireDate = employee.HireDate;
            contextEmployee.HomePhone = employee.HomePhone;
            contextEmployee.Notes = employee.Notes;
            contextEmployee.Photo = employee.Photo;
            contextEmployee.PhotoPath = employee.PhotoPath;
            contextEmployee.PostalCode = employee.PostalCode;
            contextEmployee.Region = employee.Region;
            contextEmployee.ReportsTo = employee.ReportsTo;
            contextEmployee.Title = employee.Title;
            contextEmployee.TitleOfCourtesy = employee.TitleOfCourtesy;

            await this.northwindContext.SaveChangesAsync();
            return true;
        }

        private static Employee MapEmployee(Entities.Employee employee)
        {
            return new Employee()
            {
                EmployeeID = employee.EmployeeID,
                Address = employee.Address,
                BirthDate = employee.BirthDate,
                City = employee.City,
                Country = employee.Country,
                Extension = employee.Extension,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                HireDate = employee.HireDate,
                HomePhone = employee.HomePhone,
                Notes = employee.Notes,
                Photo = employee.Photo,
                PhotoPath = employee.PhotoPath,
                PostalCode = employee.PostalCode,
                Region = employee.Region,
                ReportsTo = employee.ReportsTo,
                Title = employee.Title,
                TitleOfCourtesy = employee.TitleOfCourtesy,
            };
        }

        private static Entities.Employee MapEmployee(Employee employee)
        {
            return new Entities.Employee()
            {
                EmployeeID = employee.EmployeeID,
                Address = employee.Address,
                BirthDate = employee.BirthDate,
                City = employee.City,
                Country = employee.Country,
                Extension = employee.Extension,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                HireDate = employee.HireDate,
                HomePhone = employee.HomePhone,
                Notes = employee.Notes,
                Photo = employee.Photo,
                PhotoPath = employee.PhotoPath,
                PostalCode = employee.PostalCode,
                Region = employee.Region,
                ReportsTo = employee.ReportsTo,
                Title = employee.Title,
                TitleOfCourtesy = employee.TitleOfCourtesy,
            };
        }
    }
}
