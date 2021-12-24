using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models.ViewModels
{
    public class UpdateProduct
    {
        public string Name { get; set; }
        public string Description { get; set; }

        [Range(0.01, Double.MaxValue, ErrorMessage = "Product price must be greater than than or equal to U$ 0.01.")]
        public decimal? Price { get; set; }

        [Range(0, Int32.MaxValue, ErrorMessage = "Stock quantity must be greater than or equal to 0.")]
        public int? StockQuantity { get; set; }
    }
}