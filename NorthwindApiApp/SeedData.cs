using System;
using System.Linq;
using Bogus;
using Northwind.Services.InMemory;
using Northwind.Services.InMemory.Entities;

namespace NorthwindApiApp
{
    public class SeedData
    {
        private readonly NorthwindContext northwindContext;

        public SeedData(NorthwindContext northwindContext)
        {
            if (northwindContext is null)
            {
                throw new ArgumentNullException(nameof(northwindContext));
            }

            this.northwindContext = northwindContext;
        }

        public void SeedDatabase()
        {
            this.northwindContext.Products.AddRange(new Faker<Product>()
                .RuleFor(p => p.Id, f => f.IndexFaker + 1)
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.SupplierId, f => f.Random.Int(1, 10).OrNull(f))
                .RuleFor(p => p.CategoryId, f => f.Random.Int(1, 10).OrNull(f))
                .RuleFor(p => p.UnitPrice, f => f.Random.Int(1, 500).OrNull(f))
                .RuleFor(p => p.UnitsInStock, f => f.Random.Short(0, 150).OrNull(f))
                .RuleFor(p => p.UnitsOnOrder, f => f.Random.Short(0, 100).OrNull(f))
                .RuleFor(p => p.ReorderLevel, f => f.Random.Short(0, 100).OrNull(f))
                .RuleFor(p => p.Discontinued, f => f.Random.Bool())
                .GenerateBetween(50, 100));

            this.northwindContext.ProductCategories.AddRange(new Faker<ProductCategory>()
                .RuleFor(c => c.Id, f => f.IndexFaker + 1)
                .RuleFor(c => c.Name, f => f.Commerce.Categories(1).First())
                .RuleFor(c => c.Description, f => f.Commerce.ProductDescription())
                .GenerateBetween(5, 15));

            this.northwindContext.Employees.AddRange(new Faker<Employee>("en")
                .RuleFor(e => e.EmployeeID, f => f.IndexFaker + 1)
                .RuleFor(e => e.FirstName, f => f.Name.FirstName())
                .RuleFor(e => e.LastName, f => f.Name.LastName())
                .RuleFor(e => e.BirthDate, f => f.Person.DateOfBirth.OrNull(f))
                .RuleFor(e => e.Address, f => f.Address.SecondaryAddress().OrNull(f))
                .RuleFor(e => e.City, f => f.Address.City().OrNull(f))
                .RuleFor(e => e.Country, f => f.Address.Country().OrNull(f))
                .RuleFor(e => e.Region, f => f.Address.StateAbbr().OrNull(f))
                .RuleFor(e => e.PostalCode, f => f.Address.CountryCode().OrNull(f))
                .RuleFor(e => e.HomePhone, f => f.Phone.PhoneNumber().OrNull(f))
                .GenerateBetween(5, 10));

            this.northwindContext.SaveChanges();
        }
    }
}
