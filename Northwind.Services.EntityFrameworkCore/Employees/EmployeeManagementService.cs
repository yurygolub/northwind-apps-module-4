using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public int CreateEmployee(Employee employee)
        {
            if (employee is null)
            {
                throw new ArgumentNullException(nameof(employee));
            }

            using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);
            db.Employees.Add(MapEmployee(employee));
            db.SaveChanges();
            return employee.EmployeeID;
        }

        public bool DestroyEmployee(int employeeId)
        {
            using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);

            var employee = db.Employees.Find(employeeId);
            if (employee != null)
            {
                db.Employees.Remove(employee);
                db.SaveChanges();
                return true;
            }

            return false;
        }

        public async IAsyncEnumerable<Employee> GetEmployeesAsync(int offset, int limit)
        {
            await using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);
            var employees = await Task.Run(() => GetEmployees(db, offset, limit));
            foreach (var employee in employees)
            {
                yield return employee;
            }

            static IEnumerable<Employee> GetEmployees(Context.NorthwindContext db, int offset, int limit)
            {
                return db.Employees
                    .Skip(offset)
                    .Take(limit)
                    .Select(e => MapEmployee(e));
            }
        }

        public bool TryShowEmployee(int employeeId, out Employee employee)
        {
            using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);

            var contextEmployee = db.Employees.Find(employeeId);
            employee = null;
            if (contextEmployee is null)
            {
                return false;
            }

            employee = MapEmployee(contextEmployee);
            return true;
        }

        public bool UpdateEmployee(int employeeId, Employee employee)
        {
            if (employee is null)
            {
                throw new ArgumentNullException(nameof(employee));
            }

            using Context.NorthwindContext db = new Context.NorthwindContext(this.connectionString);

            var contextEmployee = db.Employees.Find(employeeId);
            if (contextEmployee is null)
            {
                return false;
            }

            contextEmployee = MapEmployee(employee);
            db.SaveChanges();
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
