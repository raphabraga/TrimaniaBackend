using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public class Order
    {
        public int Id { get; set; }
        public User Client { get; set; }
        public decimal TotalValue { get; set; }
        public List<Product> Items { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? CancelDate { get; set; }
        public DateTime? FinishedDate { get; set; }
        public OrderStatus Status { get; set; }
        public Order() { }

    }
}