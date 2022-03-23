using System.Data.SqlClient;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Northwind.Services;
using Northwind.Services.Employees;
using Northwind.Services.Products;
using Northwind.Services.SqlServer;
using DataAccess = Northwind.Services.DataAccess;
using EntityFramework = Northwind.Services.EntityFrameworkCore;

#pragma warning disable SA1600

namespace NorthwindApiApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddTransient<IProductManagementService, DataAccess.Products.ProductManagementService>()
                .AddTransient<IProductCategoryManagementService, DataAccess.Products.ProductCategoryManagementService>()
                .AddTransient<IEmployeeManagementService, DataAccess.Employees.EmployeeManagementService>()
                .AddScoped(s => this.Configuration.GetConnectionString("SqlConnection"))
                .AddTransient<IProductManagementService, EntityFramework.Products.ProductManagementService>()
                .AddTransient<IProductCategoryManagementService, EntityFramework.Products.ProductCategoryManagementService>()
                .AddTransient<IEmployeeManagementService, EntityFramework.Employees.EmployeeManagementService>()
                .AddScoped(s =>
                {
                    string connecionString = this.Configuration.GetConnectionString("SqlConnection");
                    return new SqlConnection(connecionString);
                })
                .AddTransient<NorthwindDataAccessFactory, SqlServerDataAccessFactory>()
                .AddControllers();
        }
    }
}
