using System.ComponentModel.DataAnnotations;

namespace Backend.Models.ViewModels
{
    public class Payment
    {
        [EnumDataType(typeof(PaymentMethod), ErrorMessage = "A valid payment method must be provided")]
        [Required(ErrorMessage = "A payment method must be provided")]
        public PaymentMethod? PaymentMethod { get; set; }
    }
}