using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Backend.Models.ViewModels
{
    public class ReportFilter
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<string> UserFilter { get; set; }
        public List<OrderStatus> StatusFilter { get; set; }
    }
}