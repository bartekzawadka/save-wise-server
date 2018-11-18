using System.Collections.Generic;
using SaveWise.DataLayer.Models;

namespace SaveWise.BusinessLogic.Common
{
    public class PredefinedCategories
    {
        public IEnumerable<IncomeCategory> IncomeCategories { get; set; }

        public IEnumerable<ExpenseCategory> ExpenseCategories { get; set; }
    }
}