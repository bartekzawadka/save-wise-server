using System;
using System.ComponentModel.DataAnnotations;

namespace SaveWise.DataLayer.Models
{
    public class Expense
    {
        public string Id { get; set; }

        public DateTime? Date { get; set; }

        public float Amount { get; set; }

        public float PlannedAmount { get; set; }

        [Required(ErrorMessage = "Kategoria wydatku jest wymagana")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Type kategorii wydatku jest wymagana")]
        public string Type { get; set; }

        public string Comment { get; set; }
    }
}