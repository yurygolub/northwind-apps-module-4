using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Northwind.DataAccess;
using Northwind.DataAccess.Employees;
using Northwind.Services.Employees;

namespace Northwind.Services.DataAccess.Employees
{
    /// <summary>
    /// Represents a management service for employees.
    /// </summary>
    public class EmployeeManagementService : IEmployeeManagementService
    {
        private readonly IEmployeeDataAccessObject dataAccessObject;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeManagementService"/> class.
        /// </summary>
        /// <param name="northwindDataAccessFactory">Factory for creating Northwind DAO.</param>
        /// <param name="mapper">Mapper for entity mapping.</param>
        public EmployeeManagementService(NorthwindDataAccessFactory northwindDataAccessFactory, IMapper mapper)
        {
            if (northwindDataAccessFactory is null)
            {
                throw new ArgumentNullException(nameof(northwindDataAccessFactory));
            }

            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            this.dataAccessObject = northwindDataAccessFactory.GetEmployeeDataAccessObject();
        }

        /// <inheritdoc/>
        public async Task<int> CreateEmployeeAsync(Employee employee)
        {
            if (employee is null)
            {
                throw new ArgumentNullException(nameof(employee));
            }

            return await this.dataAccessObject.InsertEmployeeAsync(this.mapper.Map<EmployeeTransferObject>(employee));
        }

        /// <inheritdoc/>
        public async Task<bool> DestroyEmployeeAsync(int employeeId)
        {
            if (await this.dataAccessObject.DeleteEmployeeAsync(employeeId))
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<Employee> GetEmployeesAsync(int offset, int limit)
        {
            var employees = this.dataAccessObject.SelectEmployeesAsync(offset, limit);
            await foreach (var employee in employees)
            {
                yield return this.mapper.Map<Employee>(employee);
            }
        }

        /// <inheritdoc/>
        public async Task<Employee> GetEmployeeAsync(int employeeId)
        {
            try
            {
                var employeeTransferObject = await this.dataAccessObject.FindEmployeeAsync(employeeId);
                return this.mapper.Map<Employee>(employeeTransferObject);
            }
            catch (EmployeeNotFoundException)
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateEmployeeAsync(int employeeId, Employee employee)
        {
            if (employee is null)
            {
                throw new ArgumentNullException(nameof(employee));
            }

            if (await this.dataAccessObject.UpdateEmployeeAsync(employeeId, this.mapper.Map<EmployeeTransferObject>(employee)))
            {
                return true;
            }

            return false;
        }
    }
}
