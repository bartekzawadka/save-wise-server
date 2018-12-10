using System.Collections.Generic;

namespace SaveWise.DataLayer.Models.Plans
{
    public class NewPlan
    {
        public IEnumerable<IncomeCategory> IncomeCategories { get; set; }

        public IEnumerable<ExpenseCategory> ExpenseCategories { get; set; }
    }
}