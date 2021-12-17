using System.Collections.Generic;

namespace Backend.Models.ViewModels
{
    public class SalesReport
    {
        public int FinishedOrdersAmount { get; set; }
        public int CancelledOrdersAmount { get; set; }
        public decimal orders_total_value { get; set; }
        public List<Order> Orders { get; set; }
    }
}