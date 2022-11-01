using AutoMapper;
using Northwind.Services.Employees;
using Northwind.Services.Products;

namespace Northwind.Services.EntityFrameworkCore.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<Employee, Models.Employee>().ReverseMap();
            this.CreateMap<Product, Models.Product>()
                .ForMember(m => m.ProductId, opt => opt.MapFrom(src => src.Id))
                .ForMember(m => m.ProductName, opt => opt.MapFrom(src => src.Name))
                .ReverseMap();

            this.CreateMap<ProductCategory, Models.Category>()
                .ForMember(m => m.CategoryId, opt => opt.MapFrom(src => src.Id))
                .ForMember(m => m.CategoryName, opt => opt.MapFrom(src => src.Name))
                .ReverseMap();
        }
    }
}
