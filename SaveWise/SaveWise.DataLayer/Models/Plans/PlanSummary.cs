using System;
using System.Collections.Generic;

namespace SaveWise.DataLayer.Models.Plans
{
    public class PlanSummary
    {
        public string Id { get; set; }
        
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
        
        public float IncomesSum { get; set; }

        public float ExpensesSum { get; set; }

        public IList<SumPerCategory> ExpensesPerCategory { get; set; }
        
        public IList<SumPerCategory> IncomesPerCategory { get; set; }
    }
}