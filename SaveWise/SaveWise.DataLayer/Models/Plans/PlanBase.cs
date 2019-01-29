using System;

namespace SaveWise.DataLayer.Models.Plans
{
    public abstract class PlanBase
    {
        public string Id { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
        
        public float IncomesSum { get; set; }

        public float ExpensesSum { get; set; }
    }
}