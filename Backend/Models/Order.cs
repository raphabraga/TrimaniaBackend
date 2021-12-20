using System;
using System.Collections.Generic;
using Backend.Models.ViewModels;

namespace Backend.Models
{
    public class Order
    {
        public int Id { get; set; }
        public User Client { get; set; }
        public decimal TotalValue { get; set; }
        public List<ChartItem> Items { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime CancelDate { get; set; }
        public DateTime FinishedDate { get; set; }
        public OrderStatus Status { get; set; }
        public Order() { }

    }
}