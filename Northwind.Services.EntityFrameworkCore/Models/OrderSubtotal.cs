using System;
using System.Collections.Generic;

namespace Northwind.Services.EntityFrameworkCore.Models
{
    public partial class OrderSubtotal
    {
        public int OrderId { get; set; }
        public decimal? Subtotal { get; set; }
    }
}
