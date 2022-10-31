using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.DataAccess;
using Northwind.DataAccess.SqlServer;
using Northwind.Services.Employees;
using Northwind.Services.Products;
using DataAccess = Northwind.Services.DataAccess;
using EntityFramework = Northwind.Services.EntityFrameworkCore;
using InMemory = Northwind.Services.InMemory;

namespace NorthwindApiApp
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddSqlServices(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddTransient<IProductManagementService, DataAccess.Products.ProductManagementService>()
                .AddTransient<IProductCategoryManagementService, DataAccess.Products.ProductCategoryManagementService>()
                .AddTransient<IProductCategoryPicturesService, DataAccess.Products.ProductCategoryPicturesService>()
                .AddTransient<IEmployeeManagementService, DataAccess.Employees.EmployeeManagementService>()
                .AddTransient<IEmployeePicturesService, DataAccess.Employees.EmployeePicturesService>()
                .AddScoped(s => new SqlConnection(configuration.GetConnectionString("SqlConnection")))
                .AddTransient<NorthwindDataAccessFactory, SqlServerDataAccessFactory>()
                .AddAutoMapper(typeof(DataAccess.MappingProfiles.MappingProfile));
        }

        public static IServiceCollection AddEfServices(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddTransient<IProductManagementService, EntityFramework.Products.ProductManagementService>()
                .AddTransient<IProductCategoryManagementService, EntityFramework.Products.ProductCategoryManagementService>()
                .AddTransient<IProductCategoryPicturesService, EntityFramework.Products.ProductCategoryPicturesService>()
                .AddTransient<IEmployeeManagementService, EntityFramework.Employees.EmployeeManagementService>()
                .AddTransient<IEmployeePicturesService, EntityFramework.Employees.EmployeePicturesService>()
                .AddSingleton(s => new EntityFramework.Models.NorthwindContext(configuration.GetConnectionString("SqlConnection")))
                .AddAutoMapper(typeof(EntityFramework.MappingProfiles.MappingProfile));
        }

        public static IServiceCollection AddInMemoryServices(this IServiceCollection services)
        {
            return services
                .AddTransient<IProductManagementService, InMemory.Products.ProductManagementService>()
                .AddTransient<IProductCategoryManagementService, InMemory.Products.ProductCategoryManagementService>()
                .AddTransient<IProductCategoryPicturesService, InMemory.Products.ProductCategoryPicturesService>()
                .AddTransient<IEmployeeManagementService, InMemory.Employees.EmployeeManagementService>()
                .AddTransient<IEmployeePicturesService, InMemory.Employees.EmployeePicturesService>()
                .AddSingleton<SeedData>()
                .AddDbContext<InMemory.NorthwindContext>(opt => opt.UseInMemoryDatabase("Northwind"), ServiceLifetime.Singleton)
                .AddAutoMapper(typeof(InMemory.MappingProfiles.MappingProfile));
        }
    }
}
