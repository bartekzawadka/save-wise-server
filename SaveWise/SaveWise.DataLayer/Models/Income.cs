using System.ComponentModel.DataAnnotations;

namespace SaveWise.DataLayer.Models
{
    public class Income
    {
        [Required(ErrorMessage = "Kategoria przychodu jest wymagana")]
        public string Category { get; set; }

        public float Amount { get; set; }

        public float PlannedAmount { get; set; }
    }
}