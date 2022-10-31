using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Northwind.Services.Employees;

namespace Northwind.Services.EntityFrameworkCore.Employees
{
    /// <summary>
    /// Represents a management service for employees.
    /// </summary>
    public class EmployeeManagementService : IEmployeeManagementService
    {
        private readonly Models.NorthwindContext context;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeManagementService"/> class.
        /// </summary>
        public EmployeeManagementService(Models.NorthwindContext context, IMapper mapper)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<int> CreateEmployeeAsync(Employee employee)
        {
            _ = employee ?? throw new ArgumentNullException(nameof(employee));

            await this.context.Employees.AddAsync(this.mapper.Map<Models.Employee>(employee));
            await this.context.SaveChangesAsync();
            return employee.EmployeeID;
        }

        public async Task<bool> DeleteEmployeeAsync(int employeeId)
        {
            var employee = await this.context.Employees.FindAsync(employeeId);
            if (employee != null)
            {
                this.context.Employees.Remove(employee);

                var orders = this.context.Orders.Where(order => order.Employee == employee);
                this.context.Orders.RemoveRange(orders);

                var orderDetails = orders.SelectMany(
                    o => this.context.OrderDetails.Where(orderDet => orderDet.Order == o));
                this.context.OrderDetails.RemoveRange(orderDetails);


                var empl = this.context.Employees.Include(e => e.Territories).SingleOrDefault(e => e.EmployeeId == employeeId);
                if (empl != null)
                {
                    foreach (var territory in empl.Territories
                        .Where(t => t.Employees.Contains(empl)).ToList())
                    {
                        empl.Territories.Remove(territory);
                    }
                }

                await this.context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async IAsyncEnumerable<Employee> GetEmployeesAsync(int offset, int limit)
        {
            var employees = this.context.Employees
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
            var contextEmployee = await this.context.Employees.FindAsync(employeeId);
            if (contextEmployee is null)
            {
                return null;
            }

            return this.mapper.Map<Employee>(contextEmployee);
        }

        public async Task<bool> UpdateEmployeeAsync(int employeeId, Employee employee)
        {
            _ = employee ?? throw new ArgumentNullException(nameof(employee));

            var contextEmployee = await this.context.Employees.FindAsync(employeeId);
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

            await this.context.SaveChangesAsync();
            return true;
        }
    }
}
