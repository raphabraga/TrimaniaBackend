using System;
namespace Backend.Models.ViewModels
{
    public class ReportFilter
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string UserFilter { get; set; }
        public string StatusFilter { get; set; }
    }
}