using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Backend.Models.Enums;

namespace Backend.Dtos
{
    public class ReportFilter
    {
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        public List<string> UserFilter { get; set; }
        public List<OrderStatus> StatusFilter { get; set; }
    }
}