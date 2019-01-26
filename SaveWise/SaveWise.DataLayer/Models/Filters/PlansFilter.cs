using System;
using SaveWise.DataLayer.Sys;

namespace SaveWise.DataLayer.Models.Filters
{
    public class PlansFilter : Filter<Plan>
    {
        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }
    }
}