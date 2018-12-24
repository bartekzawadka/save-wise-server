using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SaveWise.DataLayer.Models
{
    public class Plan : Document
    {
        [Required(ErrorMessage = "Data rozpoczęcia okresu rozliczeniowego jest wymagana")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "Data zakończenia okresu rozliczeniowego jest wymagana")]
        public DateTime? EndDate { get; set; }

        public IList<Expense> Expenses { get; set; }
        
        public IList<Income> Incomes { get; set; }

//        [Required(ErrorMessage = "Planowane wpływy muszą zostać podane podczas planowania budżetu")]
//        public IList<Income> PlannedIncomes { get; set; }
//        
//        [Required(ErrorMessage = "Planowane wydatki muszą zostać podane podczas planowania budżetu")]
//        public IList<Expense> PlannedExpenses { get; set; }
    }
}