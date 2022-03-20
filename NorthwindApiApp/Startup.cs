using System.Data.SqlClient;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Northwind.DataAccess;
using Northwind.DataAccess.SqlServer;
using Northwind.Services.Implementation.Products;
using Northwind.Services.Products;

namespace NorthwindApiApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddTransient<IProductManagementService, ProductManagementService>()
                .AddTransient<IProductCategoryManagementService, ProductCategoryManagementService>()
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

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
    }
}
