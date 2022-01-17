using System.ComponentModel.DataAnnotations;
using Backend.Models.Enums;

namespace Backend.Dtos
{
    public class Payment
    {
        [EnumDataType(typeof(PaymentMethod), ErrorMessage = "A valid payment method must be provided")]
        [Required(ErrorMessage = "A payment method must be provided")]
        public PaymentMethod? PaymentMethod { get; set; }
    }
}