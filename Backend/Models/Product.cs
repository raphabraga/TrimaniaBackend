using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class Product
    {
        public Product() { }
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name attribute is mandatory.")]
        public string Name { get; set; }
        public string Description { get; set; }

        [Range(0.01, Double.MaxValue, ErrorMessage = "Product price must be greater than than or equal to U$ 0.01.")]
        [Required(ErrorMessage = "Product price attribute is mandatory")]
        public decimal Price { get; set; }

        [Range(0, Int32.MaxValue, ErrorMessage = "Stock quantity must be greater than or equal to 0.")]
        [Required(ErrorMessage = "Stock quantity attribute is mandatory")]
        public int StockQuantity { get; set; }
    }
}