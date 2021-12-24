using System.Text.Json.Serialization;

namespace Backend.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PaymentMethod
    {
        CreditCard,
        BankSlip,
        InCash
    }
}