using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Northwind.Services.Employees;
using Context = Northwind.Services.EntityFrameworkCore.Models;

namespace Northwind.Services.EntityFrameworkCore.Employees
{
    /// <summary>
    /// Represents a management service for employees.
    /// </summary>
    public class EmployeeManagementService : IEmployeeManagementService
    {
        private readonly string connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeManagementService"/> class.
        /// </summary>
        public EmployeeManagementService(string connectionString)
        {
            if (connectionString is null)
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            this.connectionString = connectionString;
        }

        public async Task<int> CreateEmployeeAsync(Employee employee)
        {
            if (employee is null)
            {
                throw new ArgumentNullException(nameof(employee));
            }

            await using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);
            await db.Employees.AddAsync(MapEmployee(employee));
            await db.SaveChangesAsync();
            return employee.EmployeeID;
        }

        public async Task<bool> DestroyEmployeeAsync(int employeeId)
        {
            await using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);

            var employee = await db.Employees.FindAsync(employeeId);
            if (employee != null)
            {
                db.Employees.Remove(employee);

                var orders = db.Orders.Where(order => order.Employee == employee);
                db.Orders.RemoveRange(orders);

                var orderDetails = orders.SelectMany(
                    o => db.OrderDetails.Where(orderDet => orderDet.Order == o));
                db.OrderDetails.RemoveRange(orderDetails);


                var empl = db.Employees.Include(e => e.Territories).SingleOrDefault(e => e.EmployeeId == employeeId);
                if (empl != null)
                {
                    foreach (var territory in empl.Territories
                        .Where(t => t.Employees.Contains(empl)).ToList())
                    {
                        empl.Territories.Remove(territory);
                    }
                }

                await db.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async IAsyncEnumerable<Employee> GetEmployeesAsync(int offset, int limit)
        {
            await using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);

            var employees = db.Employees
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
            await using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);

            var contextEmployee = await db.Employees.FindAsync(employeeId);
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

            await using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);

            var contextEmployee = await db.Employees.FindAsync(employeeId);
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

            await db.SaveChangesAsync();
            return true;
        }

        private static Employee MapEmployee(Context.Employee employee)
        {
            return new Employee()
            {
                EmployeeID = employee.EmployeeId,
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

        private static Context.Employee MapEmployee(Employee employee)
        {
            return new Context.Employee()
            {
                EmployeeId = employee.EmployeeID,
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
