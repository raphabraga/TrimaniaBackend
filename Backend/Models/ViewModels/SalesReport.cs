using System.Collections.Generic;

namespace Backend.Models.ViewModels
{
    public class SalesReport
    {
        public int FinishedOrdersAmount { get; set; }
        public int CancelledOrdersAmount { get; set; }
        public decimal OrdersTotalValue { get; set; }
        public List<Order> Orders { get; set; }
    }
}