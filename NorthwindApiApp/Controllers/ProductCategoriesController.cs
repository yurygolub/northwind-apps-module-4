using Microsoft.AspNetCore.Mvc;
using Northwind.Services.Products;

namespace NorthwindApiApp.Controllers
{
    [Route("api/categories")]
    public class ProductCategoriesController : ControllerBase
    {
        private readonly IProductCategoryManagementService managementService;

        public ProductCategoriesController(IProductCategoryManagementService managementService)
        {
            this.managementService = managementService;
        }

        [HttpGet]
        public IActionResult GetProductCategories()
        {
            return this.Ok(this.managementService.ShowCategories(0, 100));
        }

        [HttpGet("{id}")]
        public IActionResult GetProductCategory(int id)
        {
            if (!this.managementService.TryShowCategory(id, out ProductCategory productCategory))
            {
                return this.NotFound();
            }

            return this.Ok(productCategory);
        }

        [HttpPost]
        public IActionResult CreateProductCategory([FromBody] ProductCategory productCategory)
        {
            this.managementService.CreateCategory(productCategory);

            return this.Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProductCategory(int id)
        {
            if (!this.managementService.DestroyCategory(id))
            {
                return this.NotFound();
            }

            return this.NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProductCategory(int id, [FromBody] ProductCategory productCategory)
        {
            if (!this.managementService.UpdateCategory(id, productCategory))
            {
                return this.NotFound();
            }

            return this.NoContent();
        }
    }
}
