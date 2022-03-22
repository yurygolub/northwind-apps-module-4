using Microsoft.AspNetCore.Mvc;
using Northwind.Services.Products;

#pragma warning disable SA1600

namespace NorthwindApiApp.Controllers
{
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductManagementService managementService;

        public ProductController(IProductManagementService managementService)
        {
            this.managementService = managementService;
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            return this.Ok(this.managementService.GetProductsAsync(0, 100));
        }

        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            if (!this.managementService.TryShowProduct(id, out Product product))
            {
                return this.NotFound();
            }

            return this.Ok(product);
        }

        [HttpPost]
        public IActionResult CreateProduct([FromBody] Product product)
        {
            this.managementService.CreateProduct(product);

            return this.Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            if (!this.managementService.DestroyProduct(id))
            {
                return this.NotFound();
            }

            return this.NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] Product product)
        {
            if (!this.managementService.UpdateProduct(id, product))
            {
                return this.NotFound();
            }

            return this.NoContent();
        }
    }
}
