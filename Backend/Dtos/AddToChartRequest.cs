using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos
{
    public class AddToChartRequest
    {
        [Required(ErrorMessage = "Product ID must be provided.")]
        public int? ProductId { get; set; }

        [Required(ErrorMessage = "Product quantity must be provided.")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int? Quantity { get; set; }
    }
}