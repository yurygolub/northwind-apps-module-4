using AutoMapper;
using Northwind.Services.Employees;
using Northwind.Services.Products;

namespace Northwind.Services.InMemory.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<Employee, Entities.Employee>().ReverseMap();
            this.CreateMap<Product, Entities.Product>().ReverseMap();
            this.CreateMap<ProductCategory, Entities.ProductCategory>().ReverseMap();
        }
    }
}
