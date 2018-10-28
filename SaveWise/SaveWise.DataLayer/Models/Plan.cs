using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SaveWise.DataLayer.Models
{
    public class Plan : Document
    {
        [Required(ErrorMessage = "Miesiąc planu budżetu jest wymagany")]
        public int? Month { get; set; }
        
        [Required(ErrorMessage = "Rok planu budżetu jest wymagany")]
        public int? Year { get; set; }

        public IEnumerable<Expense> Expenses { get; set; }
    }
}