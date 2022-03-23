using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Northwind.Services.Employees;

#pragma warning disable SA1600

namespace NorthwindApiApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeManagementService managementService;

        public EmployeeController(IEmployeeManagementService managementService)
        {
            this.managementService = managementService;
        }

        [HttpGet]
        public IActionResult GetEmployees([FromQuery] int offset = 0, [FromQuery] int limit = 10)
        {
            return this.Ok(this.managementService.GetEmployeesAsync(offset, limit));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeAsync(int id)
        {
            var employee = await this.managementService.GetEmployeeAsync(id);
            if (employee is null)
            {
                return this.NotFound();
            }

            return this.Ok(employee);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployeeAsync([FromBody] Employee employee)
        {
            await this.managementService.CreateEmployeeAsync(employee);
            return this.Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeAsync(int id)
        {
            if (!await this.managementService.DestroyEmployeeAsync(id))
            {
                return this.NotFound();
            }

            return this.NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployeeAsync(int id, [FromBody] Employee employee)
        {
            if (!await this.managementService.UpdateEmployeeAsync(id, employee))
            {
                return this.NotFound();
            }

            return this.NoContent();
        }
    }
}
