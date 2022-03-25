using Microsoft.EntityFrameworkCore;
using Northwind.Services.InMemory.Entities;

namespace Northwind.Services.InMemory
{
    public class NorthwindContext : DbContext
    {
        public NorthwindContext(DbContextOptions<NorthwindContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProductCategory> ProductCategories { get; set; }

        public DbSet<Employee> Employees { get; set; }
    }
}
