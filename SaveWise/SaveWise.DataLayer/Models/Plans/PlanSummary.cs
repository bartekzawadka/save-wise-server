using System.Collections.Generic;

namespace SaveWise.DataLayer.Models.Plans
{
    public class PlanSummary : PlanBase
    {
        public IList<SumPerCategory> ExpensesPerCategory { get; set; }

        public IList<SumPerCategory> IncomesPerCategory { get; set; }
    }
}