using System.Collections.Generic;
using Backend.ViewModels;

namespace Backend.ViewModels
{
    public class SalesReport
    {
        public int FinishedOrdersAmount { get; set; }
        public int CancelledOrdersAmount { get; set; }
        public decimal OrdersTotalValue { get; set; }
        public List<ViewOrder> Orders { get; set; }
    }
}