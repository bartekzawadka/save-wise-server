using System;
using System.ComponentModel.DataAnnotations;

namespace SaveWise.DataLayer.Models
{
    public class Expense
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Data wydatku jest wymagana")]
        public DateTime? Date { get; set; }

        [Range(Double.MinValue, Double.MaxValue, ErrorMessage = "Kwota wydatku musi być większa od 0zł")]
        public float Amount { get; set; }

        [Required(ErrorMessage = "Kategoria wydatku jest wymagana")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Type kategorii wydatku jest wymagana")]
        public string Type { get; set; }

        public string Comment { get; set; }
    }
}