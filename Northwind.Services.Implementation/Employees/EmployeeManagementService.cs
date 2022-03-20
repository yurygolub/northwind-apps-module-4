﻿using System;
using System.Collections.Generic;
using System.Linq;
using Northwind.DataAccess;
using Northwind.DataAccess.Employees;
using Northwind.Services.Employees;

namespace Northwind.Services.Implementation.Employees
{
    /// <summary>
    /// Represents a management service for employees.
    /// </summary>
    public class EmployeeManagementService : IEmployeeManagementService
    {
        private readonly IEmployeeDataAccessObject dataAccessObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeManagementService"/> class.
        /// </summary>
        /// <param name="northwindDataAccessFactory">Factory for creating Northwind DAO.</param>
        public EmployeeManagementService(NorthwindDataAccessFactory northwindDataAccessFactory)
        {
            if (northwindDataAccessFactory is null)
            {
                throw new ArgumentNullException(nameof(northwindDataAccessFactory));
            }

            this.dataAccessObject = northwindDataAccessFactory.GetEmployeeDataAccessObject();
        }

        /// <inheritdoc/>
        public int CreateEmployee(Employee employee)
        {
            if (employee is null)
            {
                throw new ArgumentNullException(nameof(employee));
            }

            return this.dataAccessObject.InsertEmployee(MapEmployee(employee));
        }

        /// <inheritdoc/>
        public bool DestroyEmployee(int employeeId)
        {
            if (this.dataAccessObject.DeleteEmployee(employeeId))
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public IList<Employee> ShowEmployees(int offset, int limit)
        {
            return this.dataAccessObject
                .SelectEmployees(offset, limit)
                .Select(e => MapEmployee(e))
                .ToList();
        }

        /// <inheritdoc/>
        public bool TryShowEmployee(int employeeId, out Employee employee)
        {
            var productTransferObject = this.dataAccessObject.FindEmployee(employeeId);
            employee = MapEmployee(productTransferObject);
            if (employee is null)
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public bool UpdateEmployee(int employeeId, Employee employee)
        {
            if (this.dataAccessObject.UpdateEmployee(MapEmployee(employee)))
            {
                return true;
            }

            return false;
        }

        private static Employee MapEmployee(EmployeeTransferObject employee)
        {
            if (employee is null)
            {
                return null;
            }

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

        private static EmployeeTransferObject MapEmployee(Employee employee)
        {
            if (employee is null)
            {
                return null;
            }

            return new EmployeeTransferObject()
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
