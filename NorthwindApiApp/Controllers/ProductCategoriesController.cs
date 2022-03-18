using Microsoft.AspNetCore.Mvc;
using Northwind.Services.Products;

namespace NorthwindApiApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductCategoriesController : ControllerBase
    {
        private readonly IProductManagementService productManagementService;

        public ProductCategoriesController(IProductManagementService productManagementService)
        {
            this.productManagementService = productManagementService;
        }
    }
}
