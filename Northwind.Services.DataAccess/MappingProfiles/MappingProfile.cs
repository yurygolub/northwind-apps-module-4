using AutoMapper;
using Northwind.DataAccess.Employees;
using Northwind.DataAccess.Products;
using Northwind.Services.Employees;
using Northwind.Services.Products;

namespace Northwind.Services.DataAccess.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<Employee, EmployeeTransferObject>().ReverseMap();
            this.CreateMap<Product, ProductTransferObject>().ReverseMap();
            this.CreateMap<ProductCategory, ProductCategoryTransferObject>().ReverseMap();
        }
    }
}
