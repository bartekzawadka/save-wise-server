using System;
using SaveWise.DataLayer.Sys;

namespace SaveWise.DataLayer.Models.Filters
{
    public class ExpenseFilter : Filter<Plan>
    {
        public DateTime? DateFrom { get; set; }
        
        public DateTime? DateTo { get; set; }

        public string Category { get; set; }
    }
}