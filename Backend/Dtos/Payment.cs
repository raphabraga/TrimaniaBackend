using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Backend.Models.Enums;

namespace Backend.Dtos
{
    public class Payment
    {
        [EnumDataType(typeof(PaymentMethod))]
        [Required]
        [DisplayName("Payment method")]
        public PaymentMethod? PaymentMethod { get; set; }
    }
}