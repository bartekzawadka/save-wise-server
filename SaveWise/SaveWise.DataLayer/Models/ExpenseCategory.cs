using System.Collections.Generic;

namespace SaveWise.DataLayer.Models
{
    public class ExpenseCategory : Document
    {
        public string Name { get; set; }

        public IEnumerable<ExpenseType> Types { get; set; }
    }
}