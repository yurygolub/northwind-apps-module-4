using Microsoft.AspNetCore.Mvc;
using Northwind.Services.Employees;

#pragma warning disable SA1600

namespace NorthwindApiApp.Controllers
{
    [Route("api/employees")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeManagementService managementService;

        public EmployeeController(IEmployeeManagementService managementService)
        {
            this.managementService = managementService;
        }

        [HttpGet]
        public IActionResult GetEmployees()
        {
            return this.Ok(this.managementService.GetEmployeesAsync(0, 100));
        }

        [HttpGet("{id}")]
        public IActionResult GetEmployee(int id)
        {
            if (!this.managementService.TryShowEmployee(id, out Employee employee))
            {
                return this.NotFound();
            }

            return this.Ok(employee);
        }

        [HttpPost]
        public IActionResult CreateEmployee([FromBody] Employee employee)
        {
            this.managementService.CreateEmployee(employee);

            return this.Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteEmployee(int id)
        {
            if (!this.managementService.DestroyEmployee(id))
            {
                return this.NotFound();
            }

            return this.NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateEmployee(int id, [FromBody] Employee employee)
        {
            if (!this.managementService.UpdateEmployee(id, employee))
            {
                return this.NotFound();
            }

            return this.NoContent();
        }
    }
}
