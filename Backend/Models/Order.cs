using System;
using System.Collections.Generic;
using Backend.Models.Enums;

namespace Backend.Models
{
    public class Order
    {
        public Order() { }
        public int Id { get; set; }
        public User Client { get; set; }
        public int? ClientId { get; set; }
        public decimal TotalValue { get; set; }
        public List<CartItem> Items { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? CancellationDate { get; set; }
        public DateTime? FinishingDate { get; set; }
        public OrderStatus Status { get; set; }

    }
}