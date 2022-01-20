using System.ComponentModel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos
{
    public class UpdateProduct
    {
        public string Name { get; set; }
        public string Description { get; set; }

        [Range(0.01, Double.MaxValue, ErrorMessage = "{0} must be greater than than or equal to U$ {1}.")]
        public decimal? Price { get; set; }

        [DisplayName("Stock quantity")]
        [Range(0, Int32.MaxValue, ErrorMessage = "{0} must be greater than or equal to {1}.")]
        public int? StockQuantity { get; set; }
    }
}