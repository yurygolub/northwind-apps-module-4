using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeManagementService"/> class.
        /// </summary>
        /// <param name="northwindContext">NorthwindContext.</param>
        /// <param name="mapper">Mapper for entity mapping.</param>
        /// <exception cref="ArgumentNullException">Thrown if northwindContext is null.</exception>
        public EmployeeManagementService(NorthwindContext northwindContext, IMapper mapper)
        {
            if (northwindContext is null)
            {
                throw new ArgumentNullException(nameof(northwindContext));
            }

            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            this.northwindContext = northwindContext;
        }

        public async Task<int> CreateEmployeeAsync(Employee employee)
        {
            if (employee is null)
            {
                throw new ArgumentNullException(nameof(employee));
            }

            await this.northwindContext.Employees.AddAsync(this.mapper.Map<Entities.Employee>(employee));
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
                    .Select(e => this.mapper.Map<Employee>(e));

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

            return this.mapper.Map<Employee>(contextEmployee);
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
    }
}
