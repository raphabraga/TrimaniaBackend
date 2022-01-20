using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class Product
    {
        public Product() { }
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        [Range(0.01, Double.MaxValue, ErrorMessage = "{0} must be greater than than or equal to U$ {1}.")]
        [Required]
        public decimal? Price { get; set; }

        [DisplayName("Stock quantity")]
        [Range(0, Int32.MaxValue, ErrorMessage = "{0} must be greater than or equal to {1}.")]
        [Required]
        public int? StockQuantity { get; set; }
    }
}