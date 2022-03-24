using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Northwind.Services.Products;

#pragma warning disable SA1600

namespace NorthwindApiApp.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class ProductCategoriesController : ControllerBase
    {
        private readonly IProductCategoryManagementService managementService;
        private readonly IProductCategoryPicturesService picturesService;

        public ProductCategoriesController(IProductCategoryManagementService managementService, IProductCategoryPicturesService picturesService)
        {
            this.managementService = managementService;
            this.picturesService = picturesService;
        }

        [HttpGet]
        public IActionResult GetProductCategories([FromQuery] int offset = 0, [FromQuery] int limit = 10)
        {
            return this.Ok(this.managementService.GetCategoriesAsync(offset, limit));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductCategoryAsync(int id)
        {
            var productCategory = await this.managementService.GetCategoryAsync(id);
            if (productCategory is null)
            {
                return this.NotFound();
            }

            return this.Ok(productCategory);
        }

        [HttpGet("{id}/picture")]
        public async Task<IActionResult> GetProductCategoryPictureAsync(int id)
        {
            var picture = await this.picturesService.GetProductCategoryPictureAsync(id);
            if (picture is null)
            {
                return this.NotFound();
            }

            return this.File(picture, "image/bmp");
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductCategoryAsync([FromBody] ProductCategory productCategory)
        {
            await this.managementService.CreateCategoryAsync(productCategory);
            return this.Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductCategoryAsync(int id)
        {
            if (!await this.managementService.DestroyCategoryAsync(id))
            {
                return this.NotFound();
            }

            return this.NoContent();
        }

        [HttpDelete("{id}/picture")]
        public async Task<IActionResult> DeleteProductCategoryPictureAsync(int id)
        {
            if (!await this.picturesService.DeleteProductCategoryPictureAsync(id))
            {
                return this.NotFound();
            }

            return this.NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductCategoryAsync(int id, [FromBody] ProductCategory productCategory)
        {
            if (!await this.managementService.UpdateCategoryAsync(id, productCategory))
            {
                return this.NotFound();
            }

            return this.NoContent();
        }

        [HttpPut("{id}/picture")]
        public async Task<IActionResult> UpdateProductCategoryPictureAsync(int id, [FromBody] Stream stream)
        {
            if (!await this.picturesService.UpdateProductCategoryPictureAsync(id, stream))
            {
                return this.NotFound();
            }

            return this.NoContent();
        }
    }
}
