using System.Data.SqlClient;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Northwind.DataAccess;
using Northwind.DataAccess.SqlServer;
using Northwind.Services.Employees;
using Northwind.Services.Implementation.Employees;
using Northwind.Services.Implementation.Products;
using Northwind.Services.Products;

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
                .AddTransient<IProductManagementService, ProductManagementService>()
                .AddTransient<IProductCategoryManagementService, ProductCategoryManagementService>()
                .AddTransient<IEmployeeManagementService, EmployeeManagementService>()
                .AddScoped(s =>
                {
                    string connecionString = this.Configuration.GetConnectionString("SqlConnection");
                    SqlConnection sqlConnection = new SqlConnection(connecionString);
                    sqlConnection.Open();
                    return sqlConnection;
                })
                .AddTransient<NorthwindDataAccessFactory, SqlServerDataAccessFactory>()
                .AddControllers();
        }
    }
}
