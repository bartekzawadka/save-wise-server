using System;
using System.Collections.Generic;
using SaveWise.DataLayer.Sys;

namespace SaveWise.DataLayer.Models.Filters
{
    public class PlansFilter : Filter<Plan>
    {
        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public override List<ColumnSort> Sorting { get; set; } = new List<ColumnSort>
        {
            new ColumnSort
            {
                ColumnName = nameof(Plan.StartDate),
                IsDescending = true
            }
        };
    }
}