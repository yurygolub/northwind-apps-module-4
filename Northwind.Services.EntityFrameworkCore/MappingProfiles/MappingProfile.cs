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
                .ReverseMap();

            this.CreateMap<ProductCategory, Models.Category>()
                .ForMember(m => m.CategoryId, opt => opt.MapFrom(src => src.Id))
                .ReverseMap();
        }
    }
}
