using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos
{
    public class AddToChartRequest
    {
        [Required]
        public int? ProductId { get; set; }

        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "{0} must be greater than {1}.")]
        public int? Quantity { get; set; }
    }
}