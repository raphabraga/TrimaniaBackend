using System.ComponentModel;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Backend.Serializers;

namespace Backend.Models.ViewModels
{
    public class ViewOrder
    {
        public string ClientLogin { get; set; }
        public int OrderId { get; set; }
        public OrderStatus Status { get; set; }

        [JsonConverter(typeof(DateJsonConverter))]
        public DateTime? CreationDate { get; set; }
        public decimal TotalValue { get; set; }
        public List<ViewItem> Items { get; set; }
        public DateTime? ClosingDate { get; set; }

        public ViewOrder(Order order)
        {
            ClientLogin = order.Client.Login;
            OrderId = order.Id;
            Status = order.Status;
            CreationDate = order.CreationDate;
            TotalValue = order.TotalValue;
            if (order.FinishingDate != null)
                ClosingDate = order.FinishingDate;
            else if (order.CancellationDate != null)
            {
                ClosingDate = order.CancellationDate;
            }
            else
                ClosingDate = null;
            Items = order.Items.Select(item => new ViewItem(item)).ToList();
        }
    }
}