using System.IO;
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
        private readonly IEmployeePicturesService picturesService;

        public EmployeeController(IEmployeeManagementService managementService, IEmployeePicturesService picturesService)
        {
            this.managementService = managementService;
            this.picturesService = picturesService;
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

        [HttpGet("{id}/photo")]
        public async Task<IActionResult> GetEmployeePhotoAsync(int id)
        {
            var employee = await this.managementService.GetEmployeeAsync(id);
            if (employee is null)
            {
                return this.NotFound();
            }

            return this.File(employee.Photo[78..], "image/bmp");
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

        [HttpDelete("{id}/photo")]
        public async Task<IActionResult> DeleteEmployeePhotoAsync(int id)
        {
            if (!await this.picturesService.DeleteEmployeePictureAsync(id))
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

        [HttpPut("{id}/photo")]
        public async Task<IActionResult> UpdateProductCategoryPictureAsync(int id, [FromBody] Stream stream)
        {
            if (!await this.picturesService.UpdateEmployeePictureAsync(id, stream))
            {
                return this.NotFound();
            }

            return this.NoContent();
        }
    }
}
