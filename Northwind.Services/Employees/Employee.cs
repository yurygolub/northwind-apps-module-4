using System;

#pragma warning disable CA1819 // Properties should not return arrays

namespace Northwind.Services.Employees
{
    /// <summary>
    /// Represents an employee.
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// Gets or sets a employee identifier.
        /// </summary>
        public int EmployeeID { get; set; }

        /// <summary>
        /// Gets or sets a employee last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets a employee first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets a employee title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a employee title of courtesy.
        /// </summary>
        public string TitleOfCourtesy { get; set; }

        /// <summary>
        /// Gets or sets a employee birth date.
        /// </summary>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// Gets or sets a employee hire date.
        /// </summary>
        public DateTime? HireDate { get; set; }

        /// <summary>
        /// Gets or sets a employee address.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets a employee city.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets a employee region.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets a employee postal code.
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets a employee country.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets a employee home phone.
        /// </summary>
        public string HomePhone { get; set; }

        /// <summary>
        /// Gets or sets a employee extension.
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// Gets or sets a employee photo.
        /// </summary>
        public byte[] Photo { get; set; }

        /// <summary>
        /// Gets or sets a employee notes.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets a employee reports.
        /// </summary>
        public int? ReportsTo { get; set; }

        /// <summary>
        /// Gets or sets a employee photo path.
        /// </summary>
        public string PhotoPath { get; set; }
    }
}
