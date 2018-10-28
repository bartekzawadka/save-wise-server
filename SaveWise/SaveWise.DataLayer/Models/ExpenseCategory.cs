using System.Collections.Generic;

namespace SaveWise.DataLayer.Models
{
    public class ExpenseCategory : Category
    {
        public IEnumerable<ExpenseType> Types { get; set; }
    }
}